using System;
using Engine.Model;

namespace Engine.Interface
{
    public interface ICollectable
    {
        /// <summary>
        /// Action die aufgerufen wird, wenn das Item eingesammelt wird.
        /// </summary>
        Action<Engine, Character> OnCollect { get; set; }
    }
}