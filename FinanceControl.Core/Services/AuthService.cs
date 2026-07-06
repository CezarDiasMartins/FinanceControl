using System;
using System.Configuration;
using System.Diagnostics;
using System.Security.Claims;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Enums;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Core.Interfaces.IServices;
using FinanceControl.Core.Util;
using FinanceControl.Core.ViewModels;

namespace FinanceControl.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository usuarioRepository, IPasswordResetTokenRepository tokenRepository, IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _tokenRepository = tokenRepository;
            _emailService = emailService;
        }

        public ResultadoOperacao<User> Autenticar(LoginViewModel model, string segredoJwt)
        {
            var usuario = _usuarioRepository.ObterPorEmail(model.Email);
            if (usuario == null || !CriptografiaHelper.VerificarSenha(model.Senha, usuario.Senha))
                return ResultadoOperacao<User>.Falha("E-mail ou senha inválidos.");

            GerarJwt(usuario, segredoJwt);
            return ResultadoOperacao<User>.Ok(usuario, "Login realizado com sucesso.");
        }

        public ResultadoOperacao Cadastrar(CadastroViewModel model)
        {
            var cpf = model.CPF.SoNumeros();
            if (cpf.Length != 11)
                return ResultadoOperacao.Falha("Informe um CPF válido.");

            if (_usuarioRepository.ObterPorCpf(cpf) != null)
                return ResultadoOperacao.Falha("Já existe um usuário cadastrado com este CPF.");

            if (_usuarioRepository.ObterPorEmail(model.Email) != null)
                return ResultadoOperacao.Falha("Já existe um usuário cadastrado com este e-mail.");

            var usuario = new User
            {
                Nome = model.Nome,
                CPF = cpf,
                Email = model.Email,
                Senha = CriptografiaHelper.GerarHashSenha(model.Senha),
                Role = UserRole.User
            };

            _usuarioRepository.Adicionar(usuario);
            _usuarioRepository.Salvar();
            return ResultadoOperacao.Ok("Cadastro realizado com sucesso. Faça login para continuar.");
        }

        public ResultadoOperacao<string> SolicitarRecuperacaoSenha(string email, string urlBase)
        {
            var usuario = _usuarioRepository.ObterPorEmail(email);
            if (usuario == null)
                return ResultadoOperacao<string>.Falha("Não existe um usuário com esse e-mail cadastrado.");

            _tokenRepository.InvalidarTokensDoUsuario(usuario.Id);
            var token = new PasswordResetToken
            {
                UserId = usuario.Id,
                Token = CriptografiaHelper.GerarTokenSeguro(),
                Expiracao = DateTime.Now.AddHours(2)
            };

            _tokenRepository.Adicionar(token);
            _tokenRepository.Salvar();

            try
            {
                var link = urlBase.TrimEnd('/') + "/Auth/NovaSenha?token=" + Uri.EscapeDataString(token.Token);
                if (UsarLinkDeDesenvolvimento())
                    return ResultadoOperacao<string>.Ok(link, "Link de recuperação gerado para demonstração.");

                _emailService.EnviarRecuperacaoSenha(usuario.Email, usuario.Nome, link);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Erro ao enviar e-mail de recuperação de senha: " + ex);
                return ResultadoOperacao<string>.Falha("Não foi possível enviar o e-mail de recuperação no momento.");
            }

            return ResultadoOperacao<string>.Ok(null, "Se o e-mail existir, enviaremos as instruções de recuperação.");
        }

        private static bool UsarLinkDeDesenvolvimento()
        {
            var modo = ConfigurationManager.AppSettings["PasswordRecoveryMode"];
            return !string.Equals(modo, "Smtp", StringComparison.OrdinalIgnoreCase);
        }

        public ResultadoOperacao RedefinirSenha(NovaSenhaViewModel model)
        {
            var token = _tokenRepository.ObterPorToken(model.Token);
            if (token == null || !token.EstaValido())
                return ResultadoOperacao.Falha("Link de recuperação inválido ou expirado.");

            var usuario = _usuarioRepository.ObterPorId(token.UserId);
            if (usuario == null)
                return ResultadoOperacao.Falha("Usuário não localizado.");

            usuario.Senha = CriptografiaHelper.GerarHashSenha(model.Senha);
            usuario.DataAlteracao = DateTime.Now;
            token.Usado = true;

            _usuarioRepository.Atualizar(usuario);
            _tokenRepository.Atualizar(token);
            _usuarioRepository.Salvar();
            return ResultadoOperacao.Ok("Senha alterada com sucesso.");
        }

        public ClaimsIdentity CriarClaims(User usuario)
        {
            var identity = new ClaimsIdentity("FinanceControlJwt");
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, usuario.Nome));
            identity.AddClaim(new Claim(ClaimTypes.Email, usuario.Email));
            identity.AddClaim(new Claim(ClaimTypes.Role, usuario.Role.ToString()));
            return identity;
        }

        public string GerarJwt(User usuario, string segredoJwt)
        {
            return JwtHelper.GerarToken(usuario, segredoJwt, 480);
        }
    }
}
