using System;
using KBuildSystem.App.Configuration;

namespace KBuildSystem.App.Helpers
{
    public interface HasObjectValue
    {
        object ObjectValue { get; set; }
    }

    public interface ValueChangedObservable
    {
        event EventHandler ValueChanged;

        void UnbindAll();

        string Description { get; set; }
    }

    public class Bindable<T> : HasObjectValue, ValueChangedObservable
        where T : IComparable
    {
        private T value;

        public T Default;

        public bool IsDefault => object.Equals(value, Default);

        public event EventHandler ValueChanged;

        public virtual T Value
        {
            get { return value; }
            set
            {
                if (this.value?.CompareTo(value) == 0) return;

                this.value = value;

                TriggerChange();
            }
        }

        public Bindable()
        { }

        public Bindable(T value)
        {
            Value = value;
        }

        public static implicit operator T(Bindable<T> value)
        {
            return value.Value;
        }

        public static implicit operator Bindable<T>(T value)
        {
            return new Bindable<T>(value);
        }


        public object ObjectValue
        {
            get
            {
                return Value;
            }
            set
            {


                try
                {
                    Value = (T)value;
                }
                catch { }
            }
        }

        internal void TriggerChange()
        {
            if (ValueChanged != null) ValueChanged(this, null);
        }

        public void UnbindAll()
        {
            ValueChanged = null;
        }

        string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public override string ToString()
        {
            return value.ToString();
        }

        internal void Reset()
        {
            Value = Default;
        }
    }

    public class BindableBool : ValueChangedObservable
    {
        private bool value;
        public bool Default;

        public bool Value
        {
            get { return value; }
            set
            {
                if (value == this.value) return;

                this.value = value;
                TriggerChange();
            }
        }

        internal void TriggerChange()
        {
            if (ValueChanged != null) ValueChanged(this, null);
        }

        public BindableBool(bool value = false)
        {
            this.value = value;
        }

        public static implicit operator bool(BindableBool value)
        {
            return value == null ? false : value.Value;
        }

        public static implicit operator BindableBool(bool value)
        {
            return new BindableBool(value);
        }

        public override string ToString()
        {
            return value ? @"1" : @"0";
        }

        public void Toggle()
        {
            Value = !Value;
        }

        public void UnbindAll()
        {
            ValueChanged = null;
        }

        string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public event EventHandler ValueChanged;
    }

    public class BindableDouble : ValueChangedObservable
    {
        private double value;
        public double Default;

        internal double MinValue = double.MinValue;
        internal double MaxValue = double.MaxValue;

        public virtual double Value
        {
            get { return value; }
            set
            {
                double boundValue = value;

                if (boundValue > MaxValue)
                    boundValue = MaxValue;
                else if (boundValue < MinValue)
                    boundValue = MinValue;

                if (boundValue == this.value)
                    return;

                this.value = boundValue;
                if (ValueChanged != null) ValueChanged(this, null);
            }
        }

        public BindableDouble(double value = 0)
        {
            this.value = value;
        }

        public static implicit operator double(BindableDouble value)
        {
            return value == null ? 0 : value.Value;
        }

        public static implicit operator BindableDouble(double value)
        {
            return new BindableDouble(value);
        }

        public override string ToString()
        {
            return value.ToString("0.##", ConfigManager.numberFormat);
        }

        public void UnbindAll()
        {
            ValueChanged = null;
        }

        string description;
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public bool IsDefault
        {
            get
            {
                return value == Default;
            }
        }

        public event EventHandler ValueChanged;
    }

    public class BindableFloat : BindableDouble
    {
        public new float Value
        {
            get
            {
                return (float)base.Value;
            }
            set
            {
                base.Value = (float)value;
            }
        }

        public BindableFloat(float value = 0.0f)
            : base(value)
        {
        }

        public new float Default
        {
            get
            {
                return (float)base.Default;
            }
            set
            {
                base.Default = value;
            }
        }

        public static implicit operator float(BindableFloat value)
        {
            return (float)value.Value;
        }

        public static implicit operator BindableFloat(float value)
        {
            return new BindableFloat(value);
        }

        public override string ToString()
        {
            return Value.ToString(ConfigManager.numberFormat);
        }

        internal void Reset()
        {
            Value = (float)Default;
        }
    }

    public class BindableInt : BindableDouble
    {
        public new int Value
        {
            get
            {
                return (int)base.Value;
            }
            set
            {
                base.Value = (int)value;
            }
        }

        public BindableInt(int value = 0)
            : base(value)
        {
        }

        public new int Default
        {
            get
            {
                return (int)base.Default;
            }
            set
            {
                base.Default = value;
            }
        }

        public static implicit operator int(BindableInt value)
        {
            return (int)value.Value;
        }

        public static implicit operator BindableInt(int value)
        {
            return new BindableInt(value);
        }

        public override string ToString()
        {
            return Value.ToString(ConfigManager.numberFormat);
        }

        internal void Reset()
        {
            Value = (int)Default;
        }
    }
}
