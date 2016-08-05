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

            Assert.Equal(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataSetProperty = 123;
            Assert.Equal(c, 1);
            Assert.Equal(model.DataSetProperty, 123);

            model.DataSetProperty = 456;
            Assert.Equal(c, 2);
            Assert.Equal(model.DataSetProperty, 456);
        }

        [Fact]
        public void RaisePropertyChanged()
        {
            var c = 0;

            Assert.Equal(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataRaisePropertyChanged = 123;
            Assert.Equal(c, 1);
            Assert.Equal(model.DataRaisePropertyChanged, 123);

            model.DataRaisePropertyChanged = 456;
            Assert.Equal(c, 2);
            Assert.Equal(model.DataRaisePropertyChanged, 456);
        }

        [Fact]
        public void SetPropertySameValue()
        {
            var c = 0;

            Assert.Equal(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataSetProperty = 123;
            Assert.Equal(c, 1);

            model.DataSetProperty = 123;
            Assert.Equal(c, 1);
        }

        [Fact]
        public void RaisePropertyChangedSameValue()
        {
            var c = 0;

            Assert.Equal(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataRaisePropertyChanged = 123;
            Assert.Equal(c, 1);

            model.DataRaisePropertyChanged = 123;
            Assert.Equal(c, 1);
        }

        [Fact]
        public void NonNotif()
        {
            var c = 0;

            Assert.Equal(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.NonNotif = 123;

            Assert.Equal(c, 0);
            Assert.Equal(model.NonNotif, 123);
        }
    }
}
