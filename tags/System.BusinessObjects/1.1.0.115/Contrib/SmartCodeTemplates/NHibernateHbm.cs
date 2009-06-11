using System;
using System.Collections.Generic;
using System.Text;
using SystemBusinessObjectTemplates;
using Kontac.Net.SmartCode.Model.Templates;
using Kontac.Net.SmartCode.Model;

namespace SystemBusinessObjectTemplates
{
    /// <summary>
    /// This file is based off the original template that comes with smartcode,
    /// Needs cleaning up, like the BusinessObjectsClass generator.
    /// </summary>
    public class NHibernateHbm : ProjectTemplate
    {
        public NHibernateHbm()
        {
            CreateOutputFile = true;
            Description = "Template to generate the XML to db mapping file for use with NHibernate.";
            Name = "Hbm File";
            OutputFolder = "BusinessObjects";
        }

        public override string OutputFileName()
        {
            return Helper.ClassName (Entity.Code) + ".hbm.xml";
        }

        public override void ProduceCode()
        {

            IList<ColumnSchema> primaryKeyColumns = Table.PrimaryKeyColumns();
            IList<ColumnSchema> noPrimaryKeyColumns = Table.NoPrimaryKeyColumns();

            if (primaryKeyColumns.Count > 0)
            {
                WriteLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
                WriteLine(
                    @"<hibernate-mapping xmlns=""urn:nhibernate-mapping-2.2"" assembly=""{1}"" namespace=""{0}"">",
                    Helper.PascalCase(Project.Code), Project.Caption);
                WriteLine(@"    <class name=""{0}"" table=""{1}"" >", Helper.ClassName(Entity.Code), Table.Name);

                // Set ID information
            {
                ColumnSchema column = primaryKeyColumns[0];
                WriteLine(@"    <id name=""ID"" type=""{0}"" column=""{1}"">", column.NetDataType, column.Name);
                if (column.IsIdentity)
                {
                    WriteLine(@"        <generator class=""identity""/>");
                }
                else
                {
                    WriteLine(
@"      <generator class=""native"">
            <param name=""sequence"">{0}_ID_SEQ</param>
        </generator>", Entity.Name);
                }
                WriteLine(@"    </id>");
            }
            //Add Properties
                foreach (ColumnSchema column in noPrimaryKeyColumns)
                {
                    if (column.NetDataType == "System.String" && column.Length>0)
                        WriteLine(@"    <property name=""{0}"" column=""{1}"" type=""{2}"" not-null=""{3}"" length=""{4}""/>", Helper.PascalCase(column.Code), column.Name, column.NetDataType, (column.IsRequired).ToString().ToLower(), column.Length);
                    else
                        WriteLine(@"    <property name=""{0}"" column=""{1}"" type=""{2}"" not-null=""{3}"" />", Helper.PascalCase(column.Code), column.Name, column.NetDataType, (column.IsRequired).ToString().ToLower());

                    
                }


                //Add Many To One Relations
                foreach (ReferenceSchema inReference in Table.InReferences)
                {
                    foreach (ReferenceJoin join in inReference.Joins)
                    {
                        string propertyName = join.ChildColumn.Code;
                        WriteLine(@"    <many-to-one name=""{0}"" column=""{1}"" class=""{2}"" />", propertyName, join.ChildColumn.Name, Helper.ClassName(inReference.ParentTable.Code));
                    }
                }

                //not supported in B/O template yet ...
                /*foreach (ReferenceSchema outReference in Table.OutReferences )
                {
                    TableSchema childTable = outReference.ChildTable;

                    if (!Helper.IsManyToManyTable(childTable))
                    {
                        WriteLine(@"    <bag name=""{0}"" table=""{1}"" inverse=""true"" lazy=""true"" cascade=""delete"">", Helper.MakePlural(childTable.Code), childTable.Name);
                        foreach (ReferenceJoin join in outReference.Joins)
                        {
                            WriteLine(@"    <key column=""{0}"" />", Helper.MakeSingle(join.ChildColumn.Code));
                            WriteLine(@"    <one-to-many class=""{0}""/>", Helper.ClassName (childTable.Code));

                        }
                        WriteLine(@"    </bag>");
                    }
                    else
                    {
                        ReferenceSchema parentSchema = childTable.InReferences[1];

                        if (parentSchema.ParentTable.ObjectID == Table.ObjectID)
                        {
                            parentSchema = childTable.InReferences[0];
                        }

                        WriteLine(@"    <bag name=""{0}"" table=""{1}"">", Helper.MakePlural(parentSchema.ParentTable.Code), childTable.Name);
                        WriteLine(@"        <key column=""{0}"" />", outReference.Joins[0].ChildColumn.Code);
                        WriteLine(@"        <many-to-many class=""{0}"" column=""{1}"" />", Helper.ClassName(parentSchema.ParentTable.Code), parentSchema.Joins[0].ChildColumn.Code);
                        WriteLine(@"    </bag>");
                    }
                }*/

                W(" </class>");
                W("</hibernate-mapping>");
     
            }
            else
            {
                WriteLine("-- Entity " + Entity.Name + " has no primary key information.");
            }   
        }
    }
}
