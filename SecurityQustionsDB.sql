USE [master]
GO

/****** Object:  Database [SecurityQuestions]    Script Date: 1/30/2023 11:58:09 AM ******/
CREATE DATABASE [SecurityQuestions]
 CONTAINMENT = NONE
 ON  PRIMARY 
(NAME = N'SecurityQuestions', FILENAME = N'C:\Users\Administrator\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\SecurityQuestions.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )

 LOG ON 
(NAME = N'SecurityQuestions_log', FILENAME = N'C:\Users\Administrator\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB\SecurityQuestions.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO



USE [SecurityQuestions]
GO

/****** Object:  Table [dbo].[UsersWithAnswers]    Script Date: 1/30/2023 12:19:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[UsersWithAnswers](
	[Name] [varchar](50) NULL,
	[UserAnswer] [varchar](100) NULL,
	[Question] [varchar](100) NULL
) ON [PRIMARY]
GO


USE [SecurityQuestions]
GO

/****** Object:  StoredProcedure [dbo].[AddAnswersForUser]    Script Date: 1/30/2023 12:19:45 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[AddAnswersForUser]
	@Name varchar(50),
	@Answer varchar(100),
	@Question varchar(100)
as

set nocount on

	insert into [dbo].[UsersWithAnswers]
	(Name, UserAnswer, Question)
	values
	(@Name, @Answer, @Question)
GO

USE [SecurityQuestions]
GO

/****** Object:  StoredProcedure [dbo].[GetAnswersForUser]    Script Date: 1/30/2023 12:20:22 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[GetAnswersForUser]
	@Name varchar(50)
as
begin
	select Name, UserAnswer, Question from [dbo].[UsersWithAnswers]
	where Name = @Name
end
GO