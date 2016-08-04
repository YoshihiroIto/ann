using Ann.Foundation.Mvvm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ann.Foundation.Test.Mvvm
{
    [TestClass]
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

        [TestMethod]
        public void SetProperty()
        {
            var c = 0;

            Assert.AreEqual(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataSetProperty = 123;
            Assert.AreEqual(c, 1);

            model.DataSetProperty = 456;
            Assert.AreEqual(c, 2);
        }

        [TestMethod]
        public void RaisePropertyChanged()
        {
            var c = 0;

            Assert.AreEqual(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataRaisePropertyChanged = 123;
            Assert.AreEqual(c, 1);

            model.DataRaisePropertyChanged = 456;
            Assert.AreEqual(c, 2);
        }

        [TestMethod]
        public void SetPropertySameValue()
        {
            var c = 0;

            Assert.AreEqual(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataSetProperty = 123;
            Assert.AreEqual(c, 1);

            model.DataSetProperty = 123;
            Assert.AreEqual(c, 1);
        }

        [TestMethod]
        public void RaisePropertyChangedSameValue()
        {
            var c = 0;

            Assert.AreEqual(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.DataRaisePropertyChanged = 123;
            Assert.AreEqual(c, 1);

            model.DataRaisePropertyChanged = 123;
            Assert.AreEqual(c, 1);
        }

        [TestMethod]
        public void NonNotif()
        {
            var c = 0;

            Assert.AreEqual(c, 0);

            var model = new Model();

            model.PropertyChanged += (s, e) => c ++;

            model.NonNotif = 123;

            Assert.AreEqual(c, 0);
        }
    }
}
