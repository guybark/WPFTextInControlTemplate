using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFTextInControlTemplate
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            // Find the live TextBlock in the ControlTemplate.
            var tb = (TextBlock)GetDescendantFromName(LiveControl, "LiveTextBlock");
            if (tb != null)
            {
                // Set new text on the TextBlock in the ControlTemplate.
                tb.Text = "The text on this live TextBlock has changed.";

                // Now raise a LiveRegionChanged event.
                var peer = FrameworkElementAutomationPeer.FromElement(tb);
                if (peer != null)
                {
                    peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
                }
            }
        }

        // Helper function to find an element within the ControlTemplate.
        public static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            int count = VisualTreeHelper.GetChildrenCount(parent);
            if (count < 1)
            {
                return null;
            }

            FrameworkElement element;
            for (int i = 0; i < count; i++)
            {
                element = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (element != null)
                {
                    if (element.Name == name)
                    {
                        return element;
                    }

                    element = GetDescendantFromName(element, name);
                    if (element != null)
                    {
                        return element;
                    }
                }
            }

            return null;
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    // Class to force a TextBlock into the Control view of the UI Automation tree.
    public class UIAControlViewTextBlock : TextBlock
    {
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new UIAControlViewTextBlockAutomationPeer(this);
        }

        public class UIAControlViewTextBlockAutomationPeer : TextBlockAutomationPeer
        {
            public UIAControlViewTextBlockAutomationPeer(UIAControlViewTextBlock owner) :
                base(owner)
            { }

            protected override bool IsControlElementCore()
            {
                return true;
            }

            // For completeness, add the TextBlock to UIA's Content view too.
            protected override bool IsContentElementCore()
            {
                return true;
            }
        }
    }
}
