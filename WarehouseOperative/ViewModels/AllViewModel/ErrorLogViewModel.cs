﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using WarehouseOperative.Models.DatabaseEntities;
using WarehouseOperative.ViewModels.Abstract;

namespace WarehouseOperative.ViewModels.AllViewModel
{
    public class ErrorLogViewModel : AllViewModel<ErrorLogs>
    {
        #region Konstruktor
        public ErrorLogViewModel()
            : base("Error Log")
        {
        }
        #endregion

        #region Helpers
        public override void Load()
        {
            try
            {
                List = new ObservableCollection<ErrorLogs>(
                        from errorLog in WarehouseEntities.ErrorLogs
                        select errorLog
                    );
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        #endregion
    }
}