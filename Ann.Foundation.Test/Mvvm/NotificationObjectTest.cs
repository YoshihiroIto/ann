using Ann.Foundation.Mvvm;
using Xunit;

namespace Ann.Foundation.Test.Mvvm
{
    public class NotificationObjectTest
    {
        public class Model : NotificationObject
        {
            #region DataSetProperty

            private int _DataSetProperty;

            public int DataSetProperty
            {
                get { return _DataSetProperty; }
                set { SetProperty(ref _DataSetProperty, value); }
            }

            #endregion

            #region DataRaisePropertyChanged

            private int _DataRaisePropertyChanged;

            public int DataRaisePropertyChanged
            {
                get { return _DataRaisePropertyChanged; }
                set
                { 
                    if (_DataRaisePropertyChanged == value)
                        return;

                    _DataRaisePropertyChanged = value;
                    RaisePropertyChanged();
                }
            }

            #endregion

            public int NonNotif { get; set; }
        }

        [Fact]
        public void SetProperty()
        {
            var c = 0;

            Assert.Equal(0, c);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataSetProperty = 123;
            Assert.Equal(1, c);
            Assert.Equal(123, model.DataSetProperty);

            model.DataSetProperty = 456;
            Assert.Equal(2, c);
            Assert.Equal(456, model.DataSetProperty);
        }

        [Fact]
        public void RaisePropertyChanged()
        {
            var c = 0;

            Assert.Equal(0, c);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataRaisePropertyChanged = 123;
            Assert.Equal(1, c);
            Assert.Equal(123, model.DataRaisePropertyChanged);

            model.DataRaisePropertyChanged = 456;
            Assert.Equal(2, c);
            Assert.Equal(456, model.DataRaisePropertyChanged);
        }

        [Fact]
        public void SetPropertySameValue()
        {
            var c = 0;

            Assert.Equal(0, c);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataSetProperty = 123;
            Assert.Equal(1, c);

            model.DataSetProperty = 123;
            Assert.Equal(1, c);
        }

        [Fact]
        public void RaisePropertyChangedSameValue()
        {
            var c = 0;

            Assert.Equal(0, c);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataRaisePropertyChanged = 123;
            Assert.Equal(1, c);

            model.DataRaisePropertyChanged = 123;
            Assert.Equal(1, c);
        }

        [Fact]
        public void NonNotif()
        {
            var c = 0;

            Assert.Equal(0, c);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.NonNotif = 123;

            Assert.Equal(0, c);
            Assert.Equal(123, model.NonNotif);
        }
    }
}
