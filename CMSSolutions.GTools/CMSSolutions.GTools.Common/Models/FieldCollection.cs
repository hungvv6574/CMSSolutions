﻿using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.GTools.Common.Models
{
    public class FieldCollection : List<Field>
    {
        public Field this[string name]
        {
            get { return this.SingleOrDefault(x => x.Name == name); }
        }

        public FieldCollection()
        {
        }

        public FieldCollection(IEnumerable<Field> fields)
        {
            foreach (Field field in fields)
            {
                this.Add(field.Clone());
            }
        }
    }
}