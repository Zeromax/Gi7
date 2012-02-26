﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Gi7.Client;
using Gi7.Client.Model;
using Gi7.Client.Request;
using Gi7.Service;
using Gi7.Service.Navigation;
using Gi7.Utils;
using Microsoft.Phone.Controls;

namespace Gi7.Views
{
    public class UserViewModel : ViewModelBase
    {
        private EventsRequest _eventsRequest;
        private FollowersRequest _followersRequest;
        private FollowingsRequest _followingsRequest;
        private ObservableCollection<Repository> _repos;
        private User _user;
        private String _username;

        public UserViewModel(GithubService githubService, INavigationService navigationService, string user)
        {
            Username = user;
            EventsRequest = new EventsRequest(Username);

            RepoSelectedCommand = new RelayCommand<Repository>(r =>
            {
                if (r != null)
                    navigationService.NavigateTo(String.Format(ViewModelLocator.RepositoryUrl, r.Owner.Login, r.Name));
            });
            UserSelectedCommand = new RelayCommand<User>(u =>
            {
                if (user != null)
                    navigationService.NavigateTo(string.Format(ViewModelLocator.UserUrl, u.Login));
            });
            PivotChangedCommand = new RelayCommand<SelectionChangedEventArgs>(args =>
            {
                var header = (args.AddedItems[0] as PivotItem).Header as String;
                switch (header)
                {
                case "Feed":
                    if (EventsRequest == null)
                        EventsRequest = new EventsRequest(Username);
                    break;
                case "Repos":
                    if (Repos == null)
                    {
                        Repos = githubService.Load(new WatchedRepoRequest(Username));
                        Repos.CollectionChanged += (sender, e) =>
                        {
                            RaisePropertyChanged("WatchedRepos");
                            RaisePropertyChanged("OwnedRepos");
                        };
                    }
                    break;
                case "Follower":
                    if (FollowersRequest == null)
                        FollowersRequest = new FollowersRequest(Username);
                    break;
                case "Following":
                    if (FollowingsRequest == null)
                        FollowingsRequest = new FollowingsRequest(Username);
                    break;
                case "Profile":
                case "Details":
                    if (User == null)
                        User = githubService.Load(new UserRequest(Username), u => User = u);
                    break;
                default:
                    break;
                }
            });
        }

        public String Username
        {
            get { return _username; }
            set
            {
                {
                    if (value != _username)
                    {
                        _username = value;
                        RaisePropertyChanged("Username");
                    }
                }
            }
        }

        public User User
        {
            get { return _user; }
            set
            {
                if (_user != value)
                {
                    _user = value;
                    RaisePropertyChanged("User");
                }
            }
        }

        public EventsRequest EventsRequest
        {
            get { return _eventsRequest; }
            set
            {
                if (_eventsRequest != value)
                {
                    _eventsRequest = value;
                    RaisePropertyChanged("EventsRequest");
                }
            }
        }

        public IEnumerable<Repository> OwnedRepos
        {
            get
            {
                if (Repos != null)
                    return Repos.Where(r => r.Owner.Login == Username);
                else
                    return null;
            }
        }

        public IEnumerable<Repository> WatchedRepos
        {
            get
            {
                if (Repos != null)
                    return Repos.Where(r => r.Owner.Login != Username);
                else
                    return null;
            }
        }

        public ObservableCollection<Repository> Repos
        {
            get { return _repos; }
            set
            {
                if (_repos != value)
                {
                    _repos = value;
                    RaisePropertyChanged("Repos");
                }
            }
        }

        public FollowingsRequest FollowingsRequest
        {
            get { return _followingsRequest; }
            set
            {
                if (_followingsRequest != value)
                {
                    _followingsRequest = value;
                    RaisePropertyChanged("FollowingsRequest");
                }
            }
        }

        public FollowersRequest FollowersRequest
        {
            get { return _followersRequest; }
            set
            {
                if (_followersRequest != value)
                {
                    _followersRequest = value;
                    RaisePropertyChanged("FollowersRequest");
                }
            }
        }

        public RelayCommand<User> UserSelectedCommand { get; private set; }
        public RelayCommand<Repository> RepoSelectedCommand { get; private set; }
        public RelayCommand<SelectionChangedEventArgs> PivotChangedCommand { get; private set; }
    }
}