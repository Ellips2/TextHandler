using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextHandler.Core
{
    //interface ICloseWindows
    //{
    //    Action Close { get; set; }
    //    bool CanClose();
    //}

    public class WindowCloser2
    {
    //    public static bool GetEnableWindowClosing(DependencyObject obj)
    //    {
    //        return (bool)obj.GetValue(EnableWindowClosingProperty);
    //    }

    //    public static void SetEnableWindowClosing(DependencyObject obj, bool value)
    //    {
    //        obj.SetValue(EnableWindowClosingProperty, value);
    //    }

    //    // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
    //    public static readonly DependencyProperty EnableWindowClosingProperty =
    //        DependencyProperty.RegisterAttached("EnableWindowClosing", typeof(bool), typeof(WindowCloser), new PropertyMetadata(false, OnEnableWindowClosingChanged));

    //    private static void OnEnableWindowClosingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is Window window)
    //        {
    //            window.Loaded += (s, e) =>
    //            {
    //                if (window.DataContext is ICloseWindows vm)
    //                {
    //                    vm.Close += () =>
    //                    {
    //                        window.Close();
    //                    };
    //                    window.Closing += (s, e) =>
    //                    {
    //                        e.Cancel = !vm.CanClose();
    //                    };
    //                }
    //            };
    //        }
    //    }
    }
}
