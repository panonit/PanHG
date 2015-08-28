using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.IO;
using System.Windows.Forms;
using PanHG.ViewModel;

namespace PanHG
{
    public partial class MainWindow : Window
    {
        public static RoutedCommand Hotkeys = new RoutedCommand();

        public MainWindow()
        {
            InitializeComponent();
            PanHGViewModel content = new PanHGViewModel();
            DataContext = content;
            Hotkeys.InputGestures.Add(new KeyGesture(Key.Enter));
        }    

        private void EnterCommand( object sender, ExecutedRoutedEventArgs e ) 
        {
            if (tabItem1.IsSelected)
            {
                if ((DataContext as PanHGViewModel).UpdateRepo.CanExecute(PasswordBox))
                {
                    (DataContext as PanHGViewModel).UpdateRepo.Execute(PasswordBox);
                }
            }
            else
            {
                if ((DataContext as PanHGViewModel).CloneRepo.CanExecute(PasswordBox))
                {
                    (DataContext as PanHGViewModel).CloneRepo.Execute(PasswordBox);
                }
            }
            
        }
    }
}
