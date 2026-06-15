using System.ComponentModel;

using SampleApp.Generators;

namespace SampleApp;

internal partial class ViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [PropertyChanged]
    public partial string Name { get; set; } = "";
}


//partial class ViewModel
//{
//    public partial string Name
//    {
//        get => field;
//        set
//        {
//            if (field != value)
//            {
//                field = value;
//                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
//            }
//        }
//    }
//}