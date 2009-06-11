using System;
using System.Collections.Generic;
using System.Text;
using Kontac.Net.SmartCode.Model.Templates;
using Kontac.Net.SmartCode.Model;

namespace SystemBusinessObjectTemplates
{
    public class BusinessObjectClass : ProjectTemplate
    {
        string code_namespace = "";
        string code_summary_comment = "";
        string code_class_name = "";
        string code_business_object_properties = "";

        public BusinessObjectClass()
        {
            CreateOutputFile = true;
            Description = "Generates a System.BusinessObjects C# class for use with NHibernate";
            Name = "System.BusinessObjects Class File";
            OutputFolder = "BusinessObjects";
        }

        public override string OutputFileName()
        {
            return Helper.ClassName(Entity.Code) + ".cs";
        }

        public override void ProduceCode()
        {
            IList<ColumnSchema> primaryKeyColumns = Table.PrimaryKeyColumns();
            if (primaryKeyColumns.Count == 0) //if there are no defined primary keys then exit
            {
                WriteLine("//-- Entity " + Entity.Name + " has no primary key information.");
                return;
            }

            code_namespace = Project.Code;
            code_summary_comment = string.Format("{0} : BusinessObject", Helper.ClassName(Entity.Code));
            code_class_name = Helper.ClassName(Entity.Code);
            code_business_object_properties = "";

            if (primaryKeyColumns.Count == 1)
            {
                AddProperty(primaryKeyColumns[0], true);
            }
            else
            {
                foreach (ColumnSchema column in primaryKeyColumns)
                {
                    if (column.Code.ToLower() != "id")
                        AddProperty(column, false);
                }
            }

            foreach (ColumnSchema column in Table.NoPrimaryKeyColumns())
            {
                AddProperty(column, false);
            }

            foreach (ReferenceSchema inReference in Table.InReferences)
            {
                foreach (ReferenceJoin join in inReference.Joins)
                {
                    AddLazyProperty(inReference, join);
                }
            }

            //Format business object
            string code = string.Format(BUSINESS_OBJECT_STUCTURE,
                    code_namespace,
                    code_summary_comment,
                    code_class_name,
                    code_business_object_properties);

            Write(code);
        }

        private void AddProperty(ColumnSchema column, bool isId)
        {
            code_business_object_properties +=
                string.Format(BUSINESS_OBJECT_PROPERTY, column.NetDataType.Replace("System.",""), isId ? "ID" : column.Code, code_class_name);
        }

        private void AddLazyProperty(ReferenceSchema inReference, ReferenceJoin join)
        {
            code_business_object_properties +=
                string.Format(BUSINESS_OBJECT_LAZY_PROPERTY,
                Helper.ClassName(inReference.ParentTable.Code),
                join.ChildColumn.Code,
                join.ChildColumn.Code,
                join.ChildColumn.Caption);
        }

        #region Code Templates

        private static string BUSINESS_OBJECT_PROPERTY =
@"        public virtual {0} {1}
        {{
            get {{ return GetValue<{0}>(""{1}""); }}
            set
            {{
                BeginEdit();
                SetValue(""{1}"", value);
            }}
        }}

";
       public static string BUSINESS_OBJECT_LAZY_PROPERTY =
@"        protected {0} _{1};
        public virtual {0} {2}
        {{
            get
            {{
                return _{1};
            }}
            set
            {{
                BeginEdit();
                _{1} = value;
            }}
        }}
";

        //{0} = Namespace
        //{1} = summary comment
        //{2} = Class Name
        //{3} = Business Object Properties
        private static string BUSINESS_OBJECT_STUCTURE =
@"using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace {0}
{{
    /// <summary>
    /// {1}
    /// </summary>
    public class {2} : DataObject<{2}>
    {{

{3}

    }}
}}";
    }
        #endregion
}
