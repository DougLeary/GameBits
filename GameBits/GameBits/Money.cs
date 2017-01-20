using System;
using System.Text;

namespace GameBits
{
    public class Money
    {
        public enum Unit { cp, sp, gp, ep, pp }
        public static String[] DenominationNames = { "cp", "sp", "gp", "ep", "pp" };
        public static int[] BaseValue = { 1000, 10, 200, 400, 1000 };

        public Money.Unit Denomination;
        public int _value;

        public static int InternalValue(Money.Unit Denomination)
        {
            return Money.BaseValue[(int)Denomination];
        }

        public int Amount
        {
            get { return _value / Money.InternalValue(Denomination); }
            set { _value = value * Money.InternalValue(Denomination); }
        }

        public Money(int amount, Money.Unit denomination)
        {
            Denomination = denomination;
            _value = amount * Money.InternalValue(denomination);
        }

        public Money()
            : this(1, Money.Unit.gp)
        {
        }

        public override string ToString()
        {
            return Amount.ToString() + Money.DenominationNames[(int)Denomination];
        }

        public Money ConvertTo(Money.Unit ToDenomination)
        {
            int newAmount = this.Amount * Money.InternalValue(Denomination) / Money.InternalValue(ToDenomination);
            return new Money(newAmount, Denomination);
        }
    }
}
