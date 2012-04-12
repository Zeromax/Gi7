﻿using System;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gi7.Client;
using Gi7.Client.Model;
using Gi7.Service;
using Gi7.Service.Navigation;
using Microsoft.Phone.Controls;
using Gi7.Client.Request.PullRequest;
using Microsoft.Phone.Tasks;

namespace Gi7.Views
{
    public class PullRequestViewModel : ViewModelBase
    {
        private ListComments _commentsRequest;
        private PullRequest _pullRequest;
        private String _repoName;
        private bool _showAppBar;
        private String _pullRequestName;

        public RelayCommand<SelectionChangedEventArgs> PivotChangedCommand { get; private set; }
        public RelayCommand RepoSelectedCommand { get; private set; }
        public RelayCommand ShareCommand { get; private set; }

        public PullRequestViewModel(GithubService githubService, INavigationService navigationService, string username, string repo, string number)
        {
            ShowAppBar = true;
            RepoName = String.Format("{0}/{1}", username, repo);
            PullRequestName = "Pull Request #" + number;

            PullRequest = githubService.Load(new Get(username, repo, number), pr => PullRequest = pr);

            ShareCommand = new RelayCommand(() =>
            {
                new ShareLinkTask()
                {
                    LinkUri = new Uri(PullRequest.HtmlUrl),
                    Title = "Pull Request on" + RepoName + " is on Github: " + PullRequest.Title,
                    Message = "I found this pull request on Github, you might want to see it: " + PullRequest.Body,
                }.Show();
            }, () => PullRequest != null);

            PivotChangedCommand = new RelayCommand<SelectionChangedEventArgs>(args =>
            {
                ShowAppBar = false;
                var header = (args.AddedItems[0] as PivotItem).Header as String;
                switch (header)
                {
                    case "Comments":
                        if (CommentsRequest == null)
                            CommentsRequest = new ListComments(username, repo, number);
                        break;
                    default: // main pivot
                        ShowAppBar = true;
                        break;
                }
            });

            RepoSelectedCommand = new RelayCommand(() =>
            {
                navigationService.NavigateTo(String.Format(ViewModelLocator.RepositoryUrl, username, repo));
            });
        }

        public String RepoName
        {
            get { return _repoName; }
            set
            {
                if (_repoName != value)
                {
                    _repoName = value;
                    RaisePropertyChanged("RepoName");
                }
            }
        }

        public PullRequest PullRequest
        {
            get { return _pullRequest; }
            set
            {
                if (_pullRequest != value)
                {
                    _pullRequest = value;
                    RaisePropertyChanged("PullRequest");
                    ShareCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ListComments CommentsRequest
        {
            get { return _commentsRequest; }
            set
            {
                if (_commentsRequest != value)
                {
                    _commentsRequest = value;
                    RaisePropertyChanged("CommentsRequest");
                }
            }
        }

        public String PullRequestName
        {
            get { return _pullRequestName; }
            set
            {
                if (_pullRequestName != value)
                {
                    _pullRequestName = value;
                    RaisePropertyChanged("PullRequestName");
                }
            }
        }

        public bool ShowAppBar
        {

            get { return _showAppBar; }
            set
            {
                if (value != _showAppBar)
                {
                    _showAppBar = value;
                    RaisePropertyChanged("ShowAppBar");
                }
            }
        }
    }
}