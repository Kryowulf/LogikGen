using LogikGenAPI.Model;
using System;
using System.Collections.Generic;

namespace LogikGenAPI.Resolution.Terms
{
    public abstract class BindingTerm : Term
    {
        protected IPropertyComparer Comparer { get; set; }
        protected List<Variable> Parameters { get; set; }
        
        public BindingTerm(IPropertyComparer comparer, params Variable[] parameters)
        {
            this.Comparer = comparer;
            this.Parameters = new List<Variable>(parameters);

            foreach (Variable p in parameters)
            {
                if (p.Owner == null)
                    p.Owner = this;
            }
        }

        protected bool BindArguments(params Property[] arguments)
        {
            if (this.Parameters.Count != arguments.Length)
                throw new ArgumentException("Number of arguments does not match number of variables.");

            for (int i = 0; i < arguments.Length; i++)
            {
                if (this.Parameters[i].Owner == this)
                    this.Parameters[i].Value = arguments[i];
                else if (!Comparer.ProvenEqual(this.Parameters[i].Value, arguments[i]))
                    return false;
            }

            return true;
        }
    }
}
