using System;
using System.Collections.Generic;
using System.Transactions;
using FinanceControl.Core.DTOs;
using FinanceControl.Core.Entities;
using FinanceControl.Core.Interfaces.IRepositories;
using FinanceControl.Core.Interfaces.IServices;
using FinanceControl.Core.Util;
using FinanceControl.Core.ViewModels;

namespace FinanceControl.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _usuarioRepository;
        private readonly IFinanceRepository _financeRepository;
        private readonly IPasswordResetTokenRepository _tokenRepository;

        public UserService(IUserRepository usuarioRepository, IFinanceRepository financeRepository, IPasswordResetTokenRepository tokenRepository)
        {
            _usuarioRepository = usuarioRepository;
            _financeRepository = financeRepository;
            _tokenRepository = tokenRepository;
        }

        public User ObterPorId(int id)
        {
            return _usuarioRepository.ObterPorId(id);
        }

        public IList<User> ListarTodos()
        {
            return _usuarioRepository.ListarTodos();
        }

        public PerfilViewModel ObterPerfil(int id)
        {
            var usuario = _usuarioRepository.ObterPorId(id);
            if (usuario == null)
                return null;

            return new PerfilViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                CPF = usuario.CPF.FormatarCpf(),
                Email = usuario.Email
            };
        }

        public ResultadoOperacao AtualizarPerfil(PerfilViewModel model)
        {
            var usuario = _usuarioRepository.ObterPorId(model.Id);
            if (usuario == null)
                return ResultadoOperacao.Falha("Usuário não localizado.");

            var cpf = model.CPF.SoNumeros();
            if (cpf.Length != 11)
                return ResultadoOperacao.Falha("Informe um CPF válido.");

            var usuarioComCpf = _usuarioRepository.ObterPorCpf(cpf);
            if (usuarioComCpf != null && usuarioComCpf.Id != usuario.Id)
                return ResultadoOperacao.Falha("Já existe outro usuário com este CPF.");

            var usuarioComEmail = _usuarioRepository.ObterPorEmail(model.Email);
            if (usuarioComEmail != null && usuarioComEmail.Id != usuario.Id)
                return ResultadoOperacao.Falha("Já existe outro usuário com este e-mail.");

            usuario.Nome = model.Nome;
            usuario.CPF = cpf;
            usuario.Email = model.Email;
            usuario.DataAlteracao = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(model.NovaSenha))
                usuario.Senha = CriptografiaHelper.GerarHashSenha(model.NovaSenha);

            _usuarioRepository.Atualizar(usuario);
            _usuarioRepository.Salvar();
            return ResultadoOperacao.Ok("Perfil atualizado com sucesso.");
        }

        public ResultadoOperacao Excluir(int id, int usuarioLogadoId)
        {
            var usuario = _usuarioRepository.ObterPorId(id);
            if (usuario == null)
                return ResultadoOperacao.Falha("Usuário não localizado.");

            if (usuario.Id == usuarioLogadoId && usuario.EhAdministrador())
                return ResultadoOperacao.Falha("Não é permitido excluir o próprio administrador logado.");

            using (var transaction = new TransactionScope())
            {
                _tokenRepository.ExcluirPorUsuario(usuario.Id);
                _financeRepository.ExcluirPorUsuario(usuario.Id);
                _usuarioRepository.Excluir(usuario);
                _usuarioRepository.Salvar();

                transaction.Complete();
            }

            return ResultadoOperacao.Ok("Usuário excluído com sucesso.");
        }
    }
}
