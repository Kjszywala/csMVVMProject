﻿using WarehouseOperative.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Data.Entity;

namespace WarehouseOperative.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        /// <summary>
        /// It will include collections of commends from left menu and 
        /// collection of bookmarks.
        /// </summary>
        #region Commands
        public ICommand NewInvoiceCommand
        {
            get
            {
                return new BaseCommand(() => addBookmarkCreateNew(new NewInvoiceViewModel()));
            }
        }
        public ICommand AllInvoicesCommand
        {
            get
            {
                return new BaseCommand(showAllInvoices);
            }
        }
        public ICommand ErrorLog
        {
            get
            {
                return new BaseCommand(getErrorLog);
            }
        }
        public ICommand NewProductCommand
        {
            get
            {
                return new BaseCommand(() => addBookmarkCreateNew(new NewProductViewmodel()));
            }
        }
        public ICommand AllProductsCommand
        {
            get
            {
                return new BaseCommand(showAllProducts);
            }
        }
        public ICommand AddEmployee
        {
            get
            {
                return new BaseCommand(() => addBookmarkCreateNew(new NewEmployeeViewModel()));
            }
        }
        public ICommand GetEmployees
        {
            get
            {
                return new BaseCommand(getEmployees);
            }
        }
        public ICommand getCloseCommand
        {
            get
            {
                return new BaseCommand(getClose);
            }
        }
        public ICommand InfoCommand
        {
            get
            {
                return new BaseCommand(getInfo);
            }
        }
        public ICommand CloseBookmark
        {
            get
            {
                return new BaseCommand(deleteBookmark);
            }
        }
        #endregion

        #region Buttons in left side menu
        // This is the collection of command in left menu.
        private ReadOnlyCollection<CommandViewModel> _Commands;
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                // Check if buttons from left side menu are initialized.
                // If not create list of buttons with CreateCommands() method and
                // set this list to ReadOnlyCollection because you can
                // only create ReadOnlyCollection, you can't add it.
                if (_Commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _Commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _Commands;
            }
        }
        //Here we decide which buttons are in left side menu
        private List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                // Creat buttons
                new CommandViewModel("New Product", new BaseCommand(()=>addBookmarkCreateNew(new NewProductViewmodel()))),
                new CommandViewModel("All Products", new BaseCommand(showAllProducts)),
                new CommandViewModel("New Invoice", new BaseCommand(()=>addBookmarkCreateNew(new NewInvoiceViewModel()))),
                new CommandViewModel("All Invoices", new BaseCommand(showAllInvoices)),
                new CommandViewModel("New Employee", new BaseCommand(()=>addBookmarkCreateNew(new NewEmployeeViewModel()))),
                new CommandViewModel("All Employees", new BaseCommand(getEmployees)),
                new CommandViewModel("Error Log", new BaseCommand(getErrorLog))
            };
        }
        #endregion

        #region Bookmarks
        // This is  the collection of bookmarks.
        private ObservableCollection<WorkspaceViewModel> _Workspaces;
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get
            {
                if(_Workspaces == null)
                {
                    _Workspaces = new ObservableCollection<WorkspaceViewModel>();
                    _Workspaces.CollectionChanged += this.onWorkspacesChanged;
                }
                return _Workspaces;
            }
        }
        // This methods are standards copied from Microsoft 
        private void onWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e) 
        { 
            if (e.NewItems != null && e.NewItems.Count != 0) 
                foreach (WorkspaceViewModel workspace in e.NewItems) 
                    workspace.RequestClose += this.onWorkspaceRequestClose; 
            if (e.OldItems != null && e.OldItems.Count != 0) 
                foreach (WorkspaceViewModel workspace in e.OldItems) 
                    workspace.RequestClose -= this.onWorkspaceRequestClose; 
        }
        private void onWorkspaceRequestClose(object sender, EventArgs e)
        {
            WorkspaceViewModel workspace = sender as WorkspaceViewModel; 
            //workspace.Dispos();
            this.Workspaces.Remove(workspace); 
        }
        #endregion

        #region HelpFunctions
        /// <summary>
        /// Delete current bookmark.
        /// </summary>
        private void deleteBookmark()
        {
            if (Workspaces.Count>0)
            {
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces);
                Workspaces.RemoveAt(collectionView.CurrentPosition);
            }
        }

        /// <summary>
        /// Get information about application.
        /// </summary>
        private void getInfo()
        {
            MessageBox.Show("Author: Kamil Szywala.\nThis program was made for\nWyzsza Szkola Biznesu - National Luis University\nin a purpose of end-semester project from\nC# Interfaces.");
        }

        /// <summary>
        /// Close the aplication.
        /// </summary>
        private void getClose()
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Create a bookmark method, we do not need to write this method for each bookmark
        /// instead of it we can just pass parameter to this function and then call it like below
        /// new BaseCommand(()=>addBookmark(new NewEmployeeViewModel()))
        /// </summary>
        /// <param name="workspace"></param>
        private void addBookmarkCreateNew(WorkspaceViewModel workspace)
        {
            // add bookmark to active bookmark collection.
            this._Workspaces.Add(workspace);
            this.setActiveWorkspace(workspace);
        }

        /// <summary>
        /// Show bookmark with adding new Employee.
        /// </summary>
        //private void addEmployees()
        //{
        //    NewEmployeeViewModel database = new NewEmployeeViewModel();
        //    this._Workspaces.Add(database);
        //    this.setActiveWorkspace(database);
        //}

        ///// <summary>
        ///// Show bookmark with creating new invoice.
        ///// </summary>
        //private void createInvoice()
        //{
        //    NewInvoiceViewModel newInvoiceViewModel = new NewInvoiceViewModel();
        //    this._Workspaces.Add(newInvoiceViewModel);
        //    this.setActiveWorkspace(newInvoiceViewModel);
        //}

        ///// <summary>
        ///// Show bookmark with adding new product.
        ///// </summary>
        //private void createProduct()
        //{
        //    // create new bookmark
        //    NewProductViewmodel workspace = new NewProductViewmodel();
        //    // add bookmark to active bookmark collection.
        //    this._Workspaces.Add(workspace);
        //    this.setActiveWorkspace(workspace);
        //}

        // This is function to open bookmark with all bookmarks.
        // This method when is called checks if bookmark exist, if exist making 
        // this bookmark active, if not creating a new one.
        private void showAllProducts()
        {
            // First we looking for in bookmark collection a bookmark which is all bookmarks.
            AllProductsViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is AllProductsViewModel) as AllProductsViewModel;
            // If there is no bookmarks like this, then we creating a new one.
            if(workspace == null)
            {
                workspace = new AllProductsViewModel();
                this.Workspaces.Add(workspace);
            }
            // Bookmark activation.
            this.setActiveWorkspace(workspace);
        }

        private void showAllInvoices()
        {
            AllInvoicesViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is AllInvoicesViewModel) as AllInvoicesViewModel;
            if(workspace == null)
            {
                workspace = new AllInvoicesViewModel();
                this.Workspaces.Add(workspace);
            }
            this.setActiveWorkspace(workspace);
        }

        private void getErrorLog()
        {
            ErrorLogViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is ErrorLogViewModel) as ErrorLogViewModel;
            if (workspace == null)
            {
                workspace = new ErrorLogViewModel();
                this.Workspaces.Add(workspace);
            }
            this.setActiveWorkspace(workspace);
        }

        private void getEmployees()
        {
            AllEmloyeesViewModel workspace = this.Workspaces.FirstOrDefault(vm => vm is AllEmloyeesViewModel) as AllEmloyeesViewModel;
            if (workspace == null)
            {
                workspace = new AllEmloyeesViewModel();
                this.Workspaces.Add(workspace);
            }
            this.setActiveWorkspace(workspace);
        }

        public event EventHandler RequestClose;
        public void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }


        // this is the standard method for adding setting bookmark active
        private void setActiveWorkspace(WorkspaceViewModel workspace) 
        {
            Debug.Assert(this.Workspaces.Contains(workspace));
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(this.Workspaces); 
            if (collectionView != null) 
                collectionView.MoveCurrentTo(workspace); 
        }
        #endregion
    }
}
