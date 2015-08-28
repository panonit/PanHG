using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using SenseBoard;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using System.Threading;

namespace PanHG.ViewModel
{
    public class PanHGViewModel : ViewModelBase
    {
        private string username;
        private string cloneSearchQuery;
        private string updateSearchQuery;
        private string cloneRepoPath;
        private string updateRepoPath;

        private bool isEnable;

        private ObservableCollection<CheckBoxRepos> cloneRepoList;
        private ObservableCollection<CheckBoxRepos> updateRepoList;
        private ObservableCollection<CheckBoxRepos> cloneRepoListAll;
        private ObservableCollection<CheckBoxRepos> updateRepoListAll;

        private Visibility loaderVisibility;

        private ICommand getRepos;
        private ICommand cloneSelectAll;
        private ICommand cloneClearAll;
        private ICommand updateSelectAll;
        private ICommand updateClearAll;
        private ICommand cloneFolderBrowse;
        private ICommand updateFolderBrowse;
        private ICommand cloneRepo;
        private ICommand updateRepo;
        private ICommand scanRepos;

        private BackgroundWorker worker;

        public PanHGViewModel()
        {            
            CloneRepoPath = HGSettings.Default.CloneRepo;
            updateRepoPath = HGSettings.Default.LocalRepo;
            Username = HGSettings.Default.Username;

            loaderVisibility = Visibility.Hidden;
            isEnable = true;
           
            cloneSearchQuery = string.Empty;

            cloneRepoList = new ObservableCollection<CheckBoxRepos>();
            cloneRepoListAll = new ObservableCollection<CheckBoxRepos>();
            updateRepoList = new ObservableCollection<CheckBoxRepos>();
            updateRepoListAll = new ObservableCollection<CheckBoxRepos>();

            ObservableCollection<CheckBoxRepos> tmp = HGSettings.Default.CloneRepoList;
            if(tmp != null)
                foreach(CheckBoxRepos cbr in tmp)
                {
                    cloneRepoList.Add(cbr);
                    cloneRepoListAll.Add(cbr);
                }

            tmp = HGSettings.Default.UpdateRepoList;
            if (tmp != null)
                foreach (CheckBoxRepos cbr in tmp)
                {
                    updateRepoList.Add(cbr);
                    updateRepoListAll.Add(cbr);
                }
        }

        public bool IsEnable
        {
            get
            {
                return isEnable;
            }
            set
            {
                isEnable = value;
                OnPropertyChanged("IsEnable");
            }
        }
        
        public Visibility LoaderVisibility
        {
            get
            {
                return loaderVisibility;
            }
            set
            {
                loaderVisibility = value;
                OnPropertyChanged("LoaderVisibility");
            }
        }

        public string Username
        {
            get
            {
                return username;
            }
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }

        public string CloneSearchQuery
        {
            get
            {
                return cloneSearchQuery;
            }
            set
            {
                cloneSearchQuery = value;
                if (!string.IsNullOrEmpty(cloneSearchQuery))
                {
                    CloneRepoList.Clear();
                    foreach (CheckBoxRepos cbr in cloneRepoListAll)
                    {
                        if (cbr.Name.Contains(cloneSearchQuery))
                        {
                            CloneRepoList.Add(cbr);
                        }
                    }
                }
                else
                {
                    CloneRepoList.Clear();
                    foreach (CheckBoxRepos cbr in cloneRepoListAll)
                    {
                        CloneRepoList.Add(cbr);
                    }
                }
                OnPropertyChanged("CloneSearchQuery");
            }
        }

        public ObservableCollection<CheckBoxRepos> CloneRepoList
        {
            get
            {
                return cloneRepoList;
            }
            set
            {
                cloneRepoList = value;
                OnPropertyChanged("CloneRepoList");
            }
        }

        public string UpdateSearchQuery
        {
            get
            {
                return updateSearchQuery;
            }
            set
            {
                updateSearchQuery = value;
                if (!string.IsNullOrEmpty(updateSearchQuery))
                {
                    UpdateRepoList.Clear();
                    foreach (CheckBoxRepos cbr in updateRepoListAll)
                    {
                        if (cbr.Name.Contains(updateSearchQuery))
                        {
                            UpdateRepoList.Add(cbr);
                        }
                    }
                }
                else
                {
                    UpdateRepoList.Clear();
                    foreach (CheckBoxRepos cbr in updateRepoListAll)
                    {
                        UpdateRepoList.Add(cbr);
                    }
                }
                OnPropertyChanged("UpdateSearchQuery");
            }
        }
        
        public ObservableCollection<CheckBoxRepos> UpdateRepoList
        {
            get
            {
                return updateRepoList;
            }
            set
            {
                updateRepoList = value;
                OnPropertyChanged("UpdateRepoList");
            }
        }

        public string CloneRepoPath
        {
            get
            {
                return cloneRepoPath;
            }
            set
            {
                cloneRepoPath = value;               
                OnPropertyChanged("CloneRepoPath");
            }
        }

        public string UpdateRepoPath
        {
            get
            {
                return updateRepoPath;
            }
            set
            {
                updateRepoPath = value;
                OnPropertyChanged("UpdateRepoPath");
            }
        }

        public ICommand GetRepos
        {
            get
            {
                return getRepos ?? (getRepos = new RelayCommand(param => GetReposCommandExecute(param), param => CanGetReposCommandExecute(param)));
            }
        }

        public ICommand CloneSelectAll
        {
            get
            {
                return cloneSelectAll ?? (cloneSelectAll = new RelayCommand(param => CloneSelectAllCommandExecute(param), param => CanCloneSelectAllCommandExecute(param)));
            }
        }

        public ICommand CloneClearAll
        {
            get
            {
                return cloneClearAll ?? (cloneClearAll = new RelayCommand(param => CloneClearAllCommandExecute(param), param => CanCloneClearAllCommandExecute(param)));
            }
        }

        public ICommand CloneFolderBrowse
        {
            get
            {
                return cloneFolderBrowse ?? (cloneFolderBrowse = new RelayCommand(param => CloneFolderBrowseCommandExecute(param), param => CanCloneFolderBrowseCommandExecute(param)));
            }
        }

        public ICommand CloneRepo
        {
            get
            {
                return cloneRepo ?? (cloneRepo = new RelayCommand(param => CloneRepoCommandExecute(param), param => CanCloneRepoCommandExecute(param)));
            }
        }

        public ICommand UpdateSelectAll
        {
            get
            {
                return updateSelectAll ?? (updateSelectAll = new RelayCommand(param => UpdateSelectAllCommandExecute(param), param => CanUpdateSelectAllCommandExecute(param)));
            }
        }

        public ICommand UpdateClearAll
        {
            get
            {
                return updateClearAll ?? (updateClearAll = new RelayCommand(param => UpdateClearAllCommandExecute(param), param => CanUpdateClearAllCommandExecute(param)));
            }
        }

        public ICommand UpdateFolderBrowse
        {
            get
            {
                return updateFolderBrowse ?? (updateFolderBrowse = new RelayCommand(param => UpdateFolderBrowseCommandExecute(param), param => CanUpdateFolderBrowseCommandExecute(param)));
            }
        }

        public ICommand UpdateRepo
        {
            get
            {
                return updateRepo ?? (updateRepo = new RelayCommand(param => UpdateRepoCommandExecute(param), param => CanUpdateRepoCommandExecute(param)));
            }
        }

        public ICommand ScanRepo
        {
            get
            {
                return scanRepos ?? (scanRepos = new RelayCommand(param => ScanLocalRepoCommandExecute(param), param => CanScanLocalRepoCommandExecute(param)));
            }
        }

        private bool CanGetReposCommandExecute(object param)
        {
            String loginPassword = ((PasswordBox)param).Password;
            if (string.IsNullOrEmpty(loginPassword) || string.IsNullOrEmpty(Username))
                return false;
            else
                return true;
        }

        private void GetReposCommandExecute(object param)
        {   
            CloneRepoList.Clear();
            cloneRepoListAll.Clear();
            IsEnable = false;

            worker = new BackgroundWorker();
            worker.DoWork += workerGetRepos;
            worker.RunWorkerCompleted += workerGetReposCompleted;

            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(param);
            }            
        }

        private void workerGetRepos(object sender, DoWorkEventArgs e)
        {
            String loginPassword = ((PasswordBox)e.Argument).Password;
            LoaderVisibility = Visibility.Visible;
            List<CheckBoxRepos> repos = new List<CheckBoxRepos>();
            ObservableCollection<CheckBoxRepos> tmp = new ObservableCollection<CheckBoxRepos>();
            ObservableCollection<CheckBoxRepos> tmp1 = new ObservableCollection<CheckBoxRepos>();

            repos = getRepoList(loginPassword);

            foreach (CheckBoxRepos repo in repos)
            {
                tmp.Add(repo);
                tmp1.Add(repo);
            }
                       

            System.Windows.Application.Current.Dispatcher.Invoke(
            DispatcherPriority.Normal,
            (Action)delegate()
            {
                CloneRepoList = tmp;
                cloneRepoListAll = tmp1;
                
            });


            HGSettings.Default.CloneRepoList = CloneRepoList;
            HGSettings.Default.Save();

            LoaderVisibility = Visibility.Hidden;           
        }

        private void workerGetReposCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= workerGetRepos;
            worker.RunWorkerCompleted -= workerGetReposCompleted;
            worker.Dispose();
            worker = null;
            IsEnable = true;
            LoaderVisibility = Visibility.Hidden;
        }


        private bool CanScanLocalRepoCommandExecute(object param)
        {
            return true;
        }

        private void ScanLocalRepoCommandExecute(object param)
        {
            try
            {
                LoaderVisibility = Visibility.Visible;
                IsEnable = false;

                string[] dirs = Directory.GetDirectories(@UpdateRepoPath, ".hg", SearchOption.AllDirectories);                
                UpdateRepoList = new ObservableCollection<CheckBoxRepos>();
               
                foreach (string dir in dirs)
                {                   
                    CheckBoxRepos cbr = new CheckBoxRepos();
                    cbr.Name = Directory.GetParent(dir).FullName;
                    cbr.IsChecked = false;
                    UpdateRepoList.Add(cbr);
                    updateRepoListAll.Add(cbr);
                   
                }

                IsEnable = true;
                LoaderVisibility = Visibility.Hidden;                
            }
            catch
            {
                LoaderVisibility = Visibility.Hidden;
                IsEnable = true;
                System.Windows.Forms.MessageBox.Show("ERROR - Please chose correct folder.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CanCloneSelectAllCommandExecute(object param)
        {
            if (CloneRepoList.Count == 0)
                return false;
            else
                return true;
        }

        private void CloneSelectAllCommandExecute(object param)
        {
            foreach (CheckBoxRepos checkBoxRepo in CloneRepoList)
            {
                checkBoxRepo.IsChecked = true;
            }
        }

        private bool CanCloneClearAllCommandExecute(object param)
        {
            if (CloneRepoList.Count == 0)
                return false;
            else
                return true;
        }

        private void CloneClearAllCommandExecute(object param)
        {
            foreach (CheckBoxRepos checkBoxRepo in CloneRepoList)
            {
                checkBoxRepo.IsChecked = false;
                checkBoxRepo.BackgroundColor = Colors.Transparent.ToString();
                checkBoxRepo.Tooltip = "Remote repo";
            }
        }

        private bool CanCloneFolderBrowseCommandExecute(object param)
        {
            return true;
        }

        private void CloneFolderBrowseCommandExecute(object param)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                CloneRepoPath = fbd.SelectedPath;
            }
        }

        private bool CanUpdateSelectAllCommandExecute(object param)
        {
            if (UpdateRepoList.Count == 0)
                return false;
            else
                return true;
        }

        private void UpdateSelectAllCommandExecute(object param)
        {
            foreach (CheckBoxRepos checkBoxRepo in UpdateRepoList)
            {
                checkBoxRepo.IsChecked = true;
            }
        }

        private bool CanUpdateClearAllCommandExecute(object param)
        {
            if (UpdateRepoList.Count == 0)
                return false;
            else
                return true;
        }

        private void UpdateClearAllCommandExecute(object param)
        {
            foreach (CheckBoxRepos checkBoxRepo in UpdateRepoList)
            {
                checkBoxRepo.IsChecked = false;
                checkBoxRepo.BackgroundColor = Colors.Transparent.ToString();
                checkBoxRepo.Tooltip = "Remote repo";
            }
        }

        private bool CanUpdateFolderBrowseCommandExecute(object param)
        {
            return true;
        }

        private void UpdateFolderBrowseCommandExecute(object param)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                UpdateRepoPath = fbd.SelectedPath;
            }
        }

        private bool CanCloneRepoCommandExecute(object param)
        {
            String loginPassword = ((PasswordBox)param).Password;
            if (CloneRepoList.Where(x => x.IsChecked==true).Count() == 0 || string.IsNullOrEmpty(loginPassword) || string.IsNullOrEmpty(Username))
                return false;
            else
                return true;
        }
      
        private void CloneRepoCommandExecute(object param)
        {           
            foreach (CheckBoxRepos cbr in CloneRepoList)
            {
                cbr.BackgroundColor = Colors.Transparent.ToString();
                cbr.Tooltip = "";
            }

            HGSettings.Default.Username = Username;
            HGSettings.Default.CloneRepo = CloneRepoPath;
            HGSettings.Default.CloneRepoList = CloneRepoList;
            HGSettings.Default.Save();

            IsEnable = false;

            worker = new BackgroundWorker();
            worker.DoWork += workerCloneRepos;
            worker.RunWorkerCompleted += workerCloneReposCompleted;
            
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(param);
            }
        }
       
        private void workerCloneRepos(object sender, DoWorkEventArgs e)
        {
            LoaderVisibility = Visibility.Visible;
            object param = (object)e.Argument;
            String loginPassword = ((PasswordBox)param).Password;

            string localColor;
            string localTooltip;

            foreach (CheckBoxRepos repoForClone in getSelectedReposForClone())
            {

                string pathString = CloneRepoPath + repoForClone.Name.Replace("/", "\\");
                if (!Directory.Exists(pathString))
                {
                    Directory.CreateDirectory(pathString);
                }

                string[] dirs = Directory.GetDirectories(@pathString, ".hg");
                if (dirs.Count() == 0)
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = "cmd.exe";
                    startInfo.WorkingDirectory = Directory.GetParent(pathString).Parent.FullName;
                    startInfo.WindowStyle = ProcessWindowStyle.Normal;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    startInfo.RedirectStandardOutput = true;
                    startInfo.RedirectStandardInput = true;

                    using (Process exeProcess = Process.Start(startInfo))
                    {
                        exeProcess.StandardInput.WriteLine(@"hg clone http://" + Username + ":" + loginPassword + "@example.com" + repoForClone.Name);  // You need to change this to adrress where you store your username and password 
                        exeProcess.StandardInput.WriteLine("exit");
                        exeProcess.WaitForExit();

                        string Output = exeProcess.StandardOutput.ReadToEnd();                        

                        if (Output.Contains("adding changesets"))
                        {
                            localColor = Colors.LightSeaGreen.ToString();
                            string[] words = Output.Split('\n');

                            localTooltip = words[9] + "\n" + words[10] +"\n" + words[11];
                        }
                        else if (Output.Contains("no changes found"))
                        {
                            localColor = Colors.LightGreen.ToString();
                            localTooltip = "Repo is empty. Nothing to clone";
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("ERROR - check your username/password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }
                }
                else
                {
                    localColor = Colors.Gold.ToString();
                    localTooltip = "Repo has allready been cloned. Use Update function";
                }

                System.Windows.Application.Current.Dispatcher.Invoke(
                   DispatcherPriority.Normal,
                   (Action)delegate()
                   {
                       repoForClone.BackgroundColor = localColor;
                       repoForClone.Tooltip = localTooltip;
                   });
            }
        }

        private void workerCloneReposCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= workerCloneRepos;
            worker.RunWorkerCompleted -= workerCloneReposCompleted;
            worker.Dispose();
            worker = null;
            IsEnable = true;
            LoaderVisibility = Visibility.Hidden;
        }

        private bool CanUpdateRepoCommandExecute(object param)
        {
            String loginPassword = ((PasswordBox)param).Password;
            if (UpdateRepoList.Where(x => x.IsChecked==true).Count() == 0 || string.IsNullOrEmpty(loginPassword) || string.IsNullOrEmpty(Username))
                return false;
            else
                return true;
        }

        private void UpdateRepoCommandExecute(object param)
        {
            foreach (CheckBoxRepos cbr in UpdateRepoList)
            {
                cbr.BackgroundColor = Colors.Transparent.ToString();
                cbr.Tooltip = "";
            }

            HGSettings.Default.Username = Username;
            HGSettings.Default.LocalRepo = updateRepoPath;
            HGSettings.Default.UpdateRepoList = updateRepoList;
            HGSettings.Default.Save();

            IsEnable = false;

            worker = new BackgroundWorker();
            worker.DoWork += workerUpdateRepos;
            worker.RunWorkerCompleted += workerUpdateReposCompleted;
            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync(param);
            }
        }

        private void workerUpdateRepos(object sender, DoWorkEventArgs e)
        {
            LoaderVisibility = Visibility.Visible;
            object param = (object)e.Argument;
            String loginPassword = ((PasswordBox)param).Password;

            string localColor;
            string localTooltip;

            foreach (CheckBoxRepos repoForUpdate in getSelectedReposForUpdate())
            {

                ProcessStartInfo updateProcess = new ProcessStartInfo();
                updateProcess.FileName = "cmd.exe";
                updateProcess.Arguments = "/k hg pull -u --config auth.xxx.prefix=*  --config auth.xxx.schemes=\"http\" --config auth.xxx.username=" + Username + " --config auth.xxx.password=" + loginPassword;

                updateProcess.WorkingDirectory = repoForUpdate.Name;
                updateProcess.WindowStyle = ProcessWindowStyle.Normal;
                updateProcess.UseShellExecute = false;
                updateProcess.CreateNoWindow = true;
                updateProcess.RedirectStandardOutput = true;
                updateProcess.RedirectStandardInput = true;
                updateProcess.RedirectStandardError = true;


                using (Process exeProcess = Process.Start(updateProcess))
                {
                    exeProcess.StandardInput.WriteLine("exit");
                    exeProcess.WaitForExit();

                    string Output = exeProcess.StandardOutput.ReadToEnd();                    

                    if (Output.Contains("adding changesets"))
                    {
                        localColor = Colors.LightSeaGreen.ToString();

                        string[] words = Output.Split('\n');

                        localTooltip = words[5] + "\n\r" + words[6];
                    }
                    else if (Output.Contains("no changes found"))
                    {
                        localColor = Colors.LightGreen.ToString();
                        localTooltip = "No changes found";
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("ERROR - check your username/password", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    }

                    System.Windows.Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Normal,
                     (Action)delegate()
                     {
                         repoForUpdate.BackgroundColor = localColor;
                         repoForUpdate.Tooltip = localTooltip;
                     });
                }
            }
        }

        private void workerUpdateReposCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.DoWork -= workerUpdateRepos;
            worker.RunWorkerCompleted -= workerUpdateReposCompleted;
            worker.Dispose();
            worker = null;
            IsEnable = true;
            LoaderVisibility = Visibility.Hidden;
        }

        private IEnumerable<CheckBoxRepos> getSelectedReposForClone()
        {
            foreach (CheckBoxRepos cbr in CloneRepoList)
            {
                if (cbr.IsChecked == true)
                    yield return cbr;
            }
        }

        private IEnumerable<CheckBoxRepos> getSelectedReposForUpdate()
        {
            foreach (CheckBoxRepos cbr in UpdateRepoList)
            {
                if (cbr.IsChecked == true)
                    yield return cbr;
            }
        }

        private List<CheckBoxRepos> getRepoList(string password)
        {
            LoaderVisibility = Visibility.Visible;
            List<CheckBoxRepos> repos = new List<CheckBoxRepos>();
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create("http://example.com");  // Returns a response from an Internet resource. You can use it to send HTTP requests to any server. You need to change it to send requests to your server.
                webRequest.Method = "GET";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Credentials = new NetworkCredential(username, password);

                string webpageContent = string.Empty;

                using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        while (!reader.EndOfStream)
                        {
                            string repoName = string.Empty;
                            webpageContent = reader.ReadLine();
                            int index1 = webpageContent.IndexOf("<a href=\"/");
                            if (index1 > 0)
                            {
                                int index2 = webpageContent.IndexOf("/\">");
                                repoName = webpageContent.Substring(index1 + 9, index2 - index1 - 8);

                                reader.ReadLine();
                                reader.ReadLine();
                                webpageContent = reader.ReadLine();

                                CheckBoxRepos repo = new CheckBoxRepos();
                                repo.Name = repoName;
                                repo.IsChecked = false;
                                repo.DateModified = webpageContent.Substring(21, 17);
                                repo.BackgroundColor = Colors.Transparent.ToString();
                                repo.Tooltip = "Remote repo";

                                repos.Add(repo);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            LoaderVisibility = Visibility.Hidden;

            return repos;
        }
    }
}
