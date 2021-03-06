﻿using System;
using System.IO;
using System.Threading;
using Aras.Method.Libs;
using Aras.Method.Libs.Aras.Package;
using Aras.Method.Libs.Code;
using Aras.Method.Libs.Configurations.ProjectConfigurations;
using Aras.Method.Libs.Templates;
using Aras.VS.MethodPlugin.Authentication;
using Aras.VS.MethodPlugin.Commands;
using Aras.VS.MethodPlugin.Dialogs;
using Aras.VS.MethodPlugin.Dialogs.Views;
using Aras.VS.MethodPlugin.PackageManagement;
using Aras.VS.MethodPlugin.SolutionManagement;
using Microsoft.VisualStudio.Shell.Interop;
using NSubstitute;
using NUnit.Framework;

namespace Aras.VS.MethodPlugin.Tests.Commands
{
	[TestFixture]
	[Apartment(ApartmentState.STA)]
	public class SaveToPackageCmdTest
	{
		IAuthenticationManager authManager;
		IProjectManager projectManager;
		IDialogFactory dialogFactory;
		ProjectConfigurationManager projectConfigurationManager;
		SaveToPackageCmd saveToPackageCmd;
		IVsUIShell iVsUIShell;
		ICodeProviderFactory codeProviderFactory;
		MessageManager messageManager;
		TemplateLoader templateLoader;
		static string currentPath;
		PackageManager packageManager;
		ICodeProvider codeProvider;

		[SetUp]
		public void Init()
		{
			projectManager = Substitute.For<IProjectManager>();
			projectConfigurationManager = Substitute.For<ProjectConfigurationManager>();
			dialogFactory = Substitute.For<IDialogFactory>();
			authManager = Substitute.For<IAuthenticationManager>();
			codeProviderFactory = Substitute.For<ICodeProviderFactory>();
			codeProvider = Substitute.For<ICodeProvider>();
			codeProviderFactory.GetCodeProvider(null).ReturnsForAnyArgs(codeProvider);
			messageManager = Substitute.For<MessageManager>();
			SaveToPackageCmd.Initialize(projectManager, authManager, dialogFactory, projectConfigurationManager, codeProviderFactory, messageManager);
			saveToPackageCmd = SaveToPackageCmd.Instance;
			iVsUIShell = Substitute.For<IVsUIShell>();
			currentPath = AppDomain.CurrentDomain.BaseDirectory;
			projectManager.ProjectConfigPath.Returns(Path.Combine(currentPath, "TestData\\projectConfig.xml"));
			projectConfigurationManager.Load(projectManager.ProjectConfigPath);
			projectConfigurationManager.CurrentProjectConfiguraiton.MethodConfigPath = Path.Combine(currentPath, "TestData\\method-config.xml");
			templateLoader = new TemplateLoader();
			templateLoader.Load(projectConfigurationManager.CurrentProjectConfiguraiton.MethodConfigPath);
			projectManager.MethodPath.Returns(Path.Combine(currentPath, "TestData\\TestMethod.txt"));
			packageManager = Substitute.For<PackageManager>(authManager, messageManager);
			File.Delete(Path.Combine(currentPath, "imports.mf"));
		}


		[Test]
		[Ignore("Should be updated")]
		public void ExecuteCommandImpl_ShouldReceivedGetSaveToPackageView()
		{
			// Arrange
			dialogFactory.GetSaveToPackageView(null, null, null, null, null, null, null).ReturnsForAnyArgs(new SaveToPackageViewAdapterStub());
			var messageBox = Substitute.For<IMessageBoxWindow>();
			dialogFactory.GetMessageBoxWindow().ReturnsForAnyArgs(messageBox);
			messageBox.ShowDialog(null, null, Arg.Any<MessageButtons>(), Arg.Any<MessageIcon>()).ReturnsForAnyArgs(MessageDialogResult.OK);

			//Act
			saveToPackageCmd.ExecuteCommandImpl(null, null);

			// Assert
			dialogFactory.Received().GetSaveToPackageView(Arg.Any<ProjectConfiguraiton>(), Arg.Any<TemplateLoader>(), Arg.Any<PackageManager>(), codeProvider, projectManager, Arg.Any<MethodInfo>(), Arg.Any<string>());

		}

		[Test]
		[Ignore("Should be updated")]
		public void ExecuteCommandImpl_ManifestFileShouldBeExist()
		{
			// Arrange
			dialogFactory.GetSaveToPackageView(null, null, null, null, null, null, null).ReturnsForAnyArgs(new SaveToPackageViewAdapterStub());
			var messageBox = Substitute.For<IMessageBoxWindow>();
			dialogFactory.GetMessageBoxWindow().ReturnsForAnyArgs(messageBox);
			messageBox.ShowDialog(null, null, Arg.Any<MessageButtons>(), Arg.Any<MessageIcon>()).ReturnsForAnyArgs(MessageDialogResult.OK);

			//Act
			saveToPackageCmd.ExecuteCommandImpl(null, null);

			// Assert
			Assert.IsTrue(File.Exists(Path.Combine(currentPath, "imports.mf")));
		}

		public class SaveToPackageViewAdapterStub : IViewAdaper<SaveToPackageView, SaveToPackageViewResult>
		{
			public SaveToPackageViewResult ShowDialog()
			{
				return new SaveToPackageViewResult
				{
					DialogOperationResult = true,
					MethodCode = string.Empty,
					MethodComment = string.Empty,
					SelectedIdentityId = string.Empty,
					SelectedIdentityKeyedName = string.Empty,
					SelectedPackage = new PackageInfo(string.Empty),
					MethodName = string.Empty,
					PackagePath = currentPath,
					MethodInformation = new MethodInfo
					{
						MethodLanguage = string.Empty,
						InnovatorMethodConfigId = string.Empty
					}
				};
			}
		}
	}
}
