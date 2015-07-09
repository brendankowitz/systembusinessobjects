# A Business Object Framework for .NET/C# #

Instead of attempting to reinvent yet another data access layer, this framework focuses on leveraging NHibernate 2.1 to provide useful ASP.NET focused functionality.

&lt;wiki:gadget url="http://www.ohloh.net/p/24332/widgets/project\_users\_logo.xml" height="43"  border="0" /&gt;

Some of the specific areas of interest are:
  * Databinding, handling [lazy properties](http://www.kowitz.net/archive/2007/12/19/asp.net-databinding-lazy-properties.aspx)
  * [Error handling](http://www.kowitz.net/archive/2007/11/08/idataerrorinfo-for-asp.net.aspx)
  * Implementing common interfaces and events
    * ICloneable, IEditableObject, IDataErrorInfo, INotifyPropertyChanged, INotifyPropertyChanging, NHibernate.Classic.IValidatable
    * Providing additional events: OnDeleting, OnDeleted, OnSaving, OnSaved
  * Validation
  * Object state tracking
  * Membership providers (Database agnostic)
  * ICriteria [Lambda Extensions](http://www.kowitz.net/archive/2008/08/17/what-would-nhibernate-icriteria-look-like-in-.net-3.5.aspx)
  * Class & Mapping file code generation (to kick start projects from existing schemas)
    * [T4 Templates](http://www.kowitz.net/archive/2009/07/19/create-nhibernate-classes-using-t4.aspx)
## Examples of usage ##

System.BusinessObjects.Framework
  * See BasicUsage
  * SampleConfig

System.BusinessObjects.Expressions
  * See LambdaExtensions

System.BusinessObjects.Membership
  * See SampleMembershipConfig for an example in how to add it to a project

&lt;wiki:gadget url="http://www.ohloh.net/p/24332/widgets/project\_cocomo.xml" height="240"  border="0" /&gt;


---


An older version of the code also exists (in the legacy branch) which is compatible with NHibernate 1.2.1GA.