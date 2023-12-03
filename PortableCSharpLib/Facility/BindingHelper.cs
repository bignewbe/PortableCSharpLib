using System.ComponentModel;
using System.Windows.Forms;
using CommonCSharpLibary.CustomExtensions;

namespace CommonCSharpLibary.Facility
{
    /// <summary>
    /// A helper class for creating a binding on an object that may be changed
    /// asynchronously from the bound UI thread.
    /// </summary>
    public class AsyncBindingHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// Get a binding instance that can invoke a property change
        /// on the UI thread, regardless of the originating thread
        /// </summary>
        /// <param name="bindingControl">The UI control this binding is added to</param>
        /// <param name="propertyName">The property on the UI control to bind to</param>
        /// <param name="bindingSource">The source INotifyPropertyChanged to be
        /// observed for changes</param>
        /// <param name="dataMember">The property on the source to watch</param>
        /// <returns></returns>
        public static Binding GetOneWayBinding(Control bindingControl,
                                          string propertyName,
                                          INotifyPropertyChanged bindingSource,
                                          string dataMember)
        {
            AsyncBindingHelper helper
              = new AsyncBindingHelper(bindingControl, bindingSource, dataMember, "Value1");
            return new Binding(propertyName, helper, helper.helperPropertyName);  //this create a binding between bindingControl.propertyName and helper.dataMember
        }
        public static Binding GetTwoWayBinding(Control bindingControl,
                                          string propertyName,
                                          INotifyPropertyChanged bindingSource,
                                          string dataMember)
        {
            AsyncBindingHelper helper
              = new AsyncBindingHelper(bindingControl, bindingSource, dataMember, "Value2");
            return new Binding(propertyName, helper, helper.helperPropertyName);  
        }

        Control bindingControl;
        INotifyPropertyChanged bindingSource;
        string dataMember;          //member of binding source
        string helperPropertyName;  //property within helper class, which corresponds to data member of binding source

        private AsyncBindingHelper(Control bindingControl,
                                    INotifyPropertyChanged bindingSource,
                                    string dataMember,
                                    string helperPropertyName)
        {
            this.bindingControl = bindingControl;
            this.bindingSource = bindingSource;
            this.dataMember = dataMember;
            this.helperPropertyName = helperPropertyName;
            bindingSource.PropertyChanged += new PropertyChangedEventHandler(bindingSource_PropertyChanged); //this delegate property changed event in datasource to bindingSource_PropertyChanged 
        }

        void bindingSource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null && e.PropertyName == dataMember)
            {
                bindingControl.InvokeIfRequired(c => PropertyChanged(this, new PropertyChangedEventArgs(this.helperPropertyName)));
                //if (bindingControl.InvokeRequired)
                //{
                //    bindingControl.BeginInvoke(
                //      new PropertyChangedEventHandler(bindingSource_PropertyChanged),
                //      sender,
                //      e);
                //    return;
                //}
                //PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// The current value of the data sources' datamember
        /// </summary>
        public object Value1
        {
            get
            {
                return bindingSource.GetType().GetProperty(dataMember).GetValue(bindingSource, null);
            }
        }
        /// <summary>
        /// The current value of the data sources' datamember
        /// </summary>
        public object Value2
        {
            get
            {
                return bindingSource.GetType().GetProperty(dataMember).GetValue(bindingSource, null);
            }
            set
            {
                bindingSource.GetType().GetProperty(dataMember).SetValue(bindingSource, value, null);
            }
        }
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Event fired when the dataMember property on the data source is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
