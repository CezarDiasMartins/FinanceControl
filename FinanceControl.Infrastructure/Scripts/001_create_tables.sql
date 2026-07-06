CREATE TABLE Users (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    CPF           CHAR(11)       NOT NULL UNIQUE,
    Nome          NVARCHAR(100)  NOT NULL,
    Email         NVARCHAR(150)  NOT NULL UNIQUE,
    Senha         NVARCHAR(255)  NOT NULL,
    Role          TINYINT        NOT NULL DEFAULT 2,  -- 1=Admin, 2=User
    DataInclusao  DATETIME       NOT NULL DEFAULT GETDATE(),
    DataAlteracao DATETIME       NULL
);

CREATE TABLE Finances (
    Id            INT IDENTITY(1,1) PRIMARY KEY,
    UserId        INT            NOT NULL REFERENCES Users(Id),
    Tipo          TINYINT        NOT NULL,             -- 1=Gasto, 2=Ganho
    Valor         DECIMAL(18,2)  NOT NULL,
    Descricao     NVARCHAR(255)  NOT NULL,
    DataInclusao  DATETIME       NOT NULL DEFAULT GETDATE(),
    DataAlteracao DATETIME       NULL
);

CREATE TABLE PasswordResetTokens (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UserId       INT            NOT NULL REFERENCES Users(Id),
    Token        NVARCHAR(100)  NOT NULL UNIQUE,
    Expiracao    DATETIME       NOT NULL,
    Usado        BIT            NOT NULL DEFAULT 0,
    DataInclusao DATETIME       NOT NULL DEFAULT GETDATE()
);
