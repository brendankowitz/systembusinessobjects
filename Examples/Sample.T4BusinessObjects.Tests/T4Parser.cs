using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg.MappingSchema;

namespace Sample.T4BusinessObjects.Tests
{
    public enum RelationType
    {
        Property, ManyToOne, ManyToMany
    }

    public class ClassItem
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public List<PropertyItem> Properties { get; set; }
    }

    public class PropertyItem
    {
        public string Name { get; set; }
        public string PropertyType { get; set; }
        public bool IsNullable { get; set; }

        public RelationType RelationType { get; set; }
        public string ForeignKeyClassName { get; set; }
        public string ForeignKeyID { get; set; }

        public string GetPropertyType()
        {
            return PropertyType.ToString().Replace("System.", "");
        }

        public string GetTypeCode()
        {
            return "TypeCode." + PropertyType.ToString().Replace("System.", "");
        }
    }

    public class T4Parser
    {
        public List<ClassItem> RenderClassFromHbm(string basePath, string fileName)
        {
            List<ClassItem> classList = new List<ClassItem>();
            MappingDocumentParser p = new MappingDocumentParser();
            HbmMapping m = p.Parse(System.IO.File.Open(System.IO.Path.Combine(basePath, fileName), System.IO.FileMode.Open));
            foreach (object o in m.Items)
            {
                if (o is HbmClass)
                {
                    HbmClass cl = (HbmClass)o;

                    ClassItem c = new ClassItem();
                    classList.Add(c);
                    c.Namespace = m.@namespace;
                    c.Name = cl.name;
                    c.Properties = new List<PropertyItem>();

                    if (cl.Id != null)
                    {
                        c.Properties.Add(
                                    new PropertyItem
                                    {
                                        Name = cl.Id.name,
                                        PropertyType = cl.Id.type != null ? cl.Id.type1 : "String",
                                        RelationType = RelationType.Property,
                                        IsNullable = true
                                    }
                                );
                    }

                    foreach (object po in cl.Items)
                    {
                        if (po is HbmProperty)
                        {
                            HbmProperty prop = (HbmProperty)po;
                            c.Properties.Add(
                                new PropertyItem
                                {
                                    Name = prop.name,
                                    PropertyType = prop.type1 != null ? prop.type1 : "String",
                                    RelationType = RelationType.Property,
                                    IsNullable = !prop.notnull
                                }
                            );
                        }
                        else if (po is HbmManyToOne)
                        {
                            HbmManyToOne mo = (HbmManyToOne)po;
                            c.Properties.Add(
                                new PropertyItem
                                {
                                    Name = mo.name,
                                    PropertyType = mo.@class,
                                    RelationType = RelationType.ManyToOne,
                                    IsNullable = !mo.notnull
                                }
                            );
                        }
                        else if (po is HbmSet)
                        {
                            HbmSet se = (HbmSet)po;

                            c.Properties.Add(
                                new PropertyItem
                                {
                                    Name = se.name,
                                    PropertyType = ((HbmOneToMany)se.Item).@class,
                                    RelationType = RelationType.ManyToMany
                                }
                            );
                        }
                    }
                }
            }
            return classList;
        }
    }

}
