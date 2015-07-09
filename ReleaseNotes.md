# Release Notes Details #

Release: 1.1.0.115
  * obsoleted namevaluepair in favour of KeyValuePair<>
  * Fixed error in regex validation when property is null
  * Turned dynamic-insert and dynamic-update NHibernate strategy on for Membership provider
  * Fixed they way System.BusinessObjects.Expressions/ResolveLambda.cs was resolving Properties of the parent object, or if the query is being evaluated from within the parent object.
  * Added Predicate selection in a list for .net2.0
  * Added .net2.0 collection/predicate functionality to With.cs
  * Support in System.BusinessObjects.Expressions for DetachedCriteria
  * Moved With.Transaction (obsolete) to Transaction.cs
  * Removed AutoFlush property
  * Added SaveMode to Save() property
  * Added Lambda type safe overloads to IsNull(), GetValue() and SetValue() methods
  * Updated code templates for smartcode 3
  * Added support for validation via System.ComponentModel.DataAnnotations
  * Added AddRulesFromAttributes() method to IValidationRuleCollection
  * Added projects to compile to .net2.0
  * Cleaned up some code specific to 3.5