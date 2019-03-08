﻿namespace SuperCOOL.Core
{
    public class CoolType
    {
        public CoolType Parent { get; private set; }
        public string Name { get; private set; }

        public CoolType(string Name)
        {
            this.Name = Name;
        }

        public CoolType(string Name,CoolType Parent):this(Name)
        {
            this.Parent = Parent;
        }

        public override bool Equals(object obj)
        {
            CoolType coolType = obj as CoolType;
            return coolType.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public bool IsIt(CoolType Tatara)
        {
            var type = this;
            if (type == Tatara)
                return true;
            while (type.Parent!=null)
            {
                type = type.Parent;
                if (type.Equals(Tatara)) return true;
            }
            return false;
        }

    }
}