using System;
using ReactiveUI;
using System.Reactive;
using TableJson.Models;

namespace TableJson.ViewModels
{
    public class SaveWindowViewModel : ViewModelBase
    {
        private string _QueryText = "";
        public string QueryText
        {
            get => _QueryText;
            set => this.RaiseAndSetIfChanged(ref _QueryText, value);
        }
        private string _DescriptionText = "";
        public string DescriptionText
        {
            get => _DescriptionText;
            set => this.RaiseAndSetIfChanged(ref _DescriptionText, value);
        }
        public ReactiveCommand<Unit, Unit> SaveQueryFastCommand { get; }
        public void SaveQueryFast()
        {
            var AddedQuery = new JsonQuery(QueryText, DescriptionText);
            using (var DataSource = new HelpContext())
            {
                DataSource.JsonQueryTable.Attach(AddedQuery);
                DataSource.JsonQueryTable.Add(AddedQuery);
                DataSource.SaveChanges();
            }
        }
        public SaveWindowViewModel(string query)
        {
            SaveQueryFastCommand = ReactiveCommand.Create(SaveQueryFast);
            QueryText = query;
        }
        public SaveWindowViewModel()
        {

        }
    }
}
