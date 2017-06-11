using CommonBase.UI;
using CommonBase.UI.Localization;
using CommonBase.UI.MessageDialogs;
using StoreKeeper.Client;
using StoreKeeper.Client.Objects;
using System.Windows.Input;

namespace StoreKeeper.App.ViewModels.Material
{
	public class DeleteMaterialViewModel : ViewModelBase
	{
		private readonly IDataAccess _dataAccess;
		private string _code;

		public DeleteMaterialViewModel(IDataAccess dataAccess)
		{
			_dataAccess = dataAccess;
		}

		public string Code
		{
			get { return _code; }
			set
			{
				_code = value;
				NotifyPropertyChanged("Code");
			}
		}

		public ICommand DeleteMaterialCommand
		{
			get
			{
				return new RelayCommand(ExecuteDeleteMaterialCommand, CanExecuteDeleteMaterialCommand);
			}
		}

		private bool CanExecuteDeleteMaterialCommand(object param)
		{
			if (string.IsNullOrWhiteSpace(_code))
			{
				return false;
			}

			return true;
		}

		private void ExecuteDeleteMaterialCommand(object param)
		{
			IMaterial material = _dataAccess.FindMaterial(Code);
			if (material == null)
			{
				UIApplication.MessageDialogs.Warning("MaterialNotExists".Localize());
			}
			else
			{
				if (UIApplication.MessageDialogs.Question("QuestionDeleteMaterial".Localize(), material.Code) == QuestionResult.Positive)
				{
					bool result = _dataAccess.RemoveMaterial(material);
					if (result)
					{
						UIApplication.MessageDialogs.Info("MaterialRemoved".Localize());
						Code = string.Empty;
					}
					else
					{
						UIApplication.MessageDialogs.Error("MaterialNotRemoved".Localize());
					}
				}
			}
		}
	}
}
