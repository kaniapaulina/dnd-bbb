using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DnD_BBB.Exceptions_and_Interfaces
{
    public interface IAction
    {
        void TakeDamage(int damage);
        void HealDamage(int heal);
        bool LifeStatus { get; }
    }
}
