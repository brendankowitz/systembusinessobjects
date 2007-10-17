<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Sample.UI.AddressBook._Default" %>

<%@ Register Assembly="System.BusinessObjects.Framework" Namespace="System.BusinessObjects.Validation"
    TagPrefix="cc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Brendan's Sample Address Book</title>
</head>
<body>
    <form id="form1" runat="server">
    <h1>Address Book</h1>
    <div>
        <asp:TextBox ID="TextBoxSearch" runat="server"></asp:TextBox>
        <asp:Button ID="ButtonSearch" runat="server" OnClick="ButtonSearch_Click" Text="Search" />
        <asp:Button ID="ButtonCreateNew" runat="server" OnClick="ButtonCreateNew_Click" Text="Create New" /><br />
        <br />
        <asp:GridView ID="GridViewPeople" runat="server" AllowPaging="True" AutoGenerateColumns="False"
            CellPadding="4" DataKeyNames="ID,RowState" DataSourceID="ObjectDataSourcePeopleList"
            EmptyDataText="There are no people found." ForeColor="#333333" GridLines="None"
            Width="100%" OnRowCommand="GridViewPeople_RowCommand">
            <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True"
                    SortExpression="ID" Visible="False" />
                <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
                <asp:CommandField SelectText="Addresses" ShowDeleteButton="True" ShowEditButton="True"
                    ShowSelectButton="True" ButtonType="Button" />
            </Columns>
            <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
            <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
            <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
            <EditRowStyle BackColor="#999999" />
            <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
        </asp:GridView>
    
    </div>
        <asp:ObjectDataSource ID="ObjectDataSourcePeopleList" runat="server" DataObjectTypeName="Sample.BusinessObjects.Contacts.Person"
            DeleteMethod="DeletePerson" InsertMethod="CreatePerson" OldValuesParameterFormatString="original_{0}"
            SelectMethod="SearchPeopleByName" TypeName="Sample.Facade.Controllers.ContactController"
            UpdateMethod="UpdatePerson">
            <SelectParameters>
                <asp:ControlParameter ControlID="TextBoxSearch" Name="name" PropertyName="Text" Type="String" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <asp:Panel ID="PanelAddPerson" runat="server" Visible="False">
            <asp:DetailsView ID="DetailsViewPerson" runat="server" AutoGenerateRows="False" CellPadding="4"
                DataSourceID="ObjectDataSourcePeopleList" DefaultMode="Insert" ForeColor="#333333"
                GridLines="None" OnItemCommand="DetailsViewPerson_ItemCommand" OnItemInserting="DetailsViewPerson_ItemInserting">
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <Fields>
                    <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True"
                        SortExpression="ID" />
                    <asp:BoundField DataField="FirstName" HeaderText="FirstName" SortExpression="FirstName" />
                    <asp:BoundField DataField="LastName" HeaderText="LastName" SortExpression="LastName" />
                    <asp:CommandField ShowInsertButton="True" ButtonType="Button" />
                </Fields>
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:DetailsView>
        </asp:Panel>
        <asp:Panel ID="PanelAddAddress" runat="server" Visible="False">
            <h2>Addresses</h2><br />
            <asp:Button ID="ButtonAddAddress" runat="server" Text="Create New" OnClick="ButtonAddAddress_Click" /><br />
            <asp:GridView ID="GridViewAddresses" runat="server" AutoGenerateColumns="False" CellPadding="4"
                DataSourceID="ObjectDataSourceAddress" EmptyDataText="No addresses where found"
                ForeColor="#333333" GridLines="None" Width="100%" DataKeyNames="ID,RowState">
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <Columns>
                    <asp:BoundField DataField="Address1" HeaderText="Address1" SortExpression="Address1" />
                    <asp:BoundField DataField="Suburb" HeaderText="Suburb" SortExpression="Suburb" />
                    <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" />
                    <asp:BoundField DataField="Postcode" HeaderText="Postcode" SortExpression="Postcode" />
                    <asp:CommandField ButtonType="Button" ShowDeleteButton="True" ShowEditButton="True" />
                </Columns>
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <SelectedRowStyle BackColor="#E2DED6" Font-Bold="True" ForeColor="#333333" />
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:GridView>
            <asp:ObjectDataSource ID="ObjectDataSourceAddress" runat="server" DataObjectTypeName="Sample.BusinessObjects.Contacts.Address"
                DeleteMethod="DeleteAddress" InsertMethod="CreateAddress" OldValuesParameterFormatString="original_{0}"
                SelectMethod="SearchAddresses" TypeName="Sample.Facade.Controllers.ContactController"
                UpdateMethod="UpdateAddress">
                <SelectParameters>
                    <asp:ControlParameter ControlID="GridViewPeople" Name="personID" PropertyName="SelectedValue"
                        Type="Int32" />
                </SelectParameters>
            </asp:ObjectDataSource>
            &nbsp;
            <asp:Panel ID="PanelAddNewAddress" runat="server" Visible="False">
            <asp:DetailsView ID="DetailsViewAddress" runat="server" AutoGenerateRows="False"
                CellPadding="4" DataSourceID="ObjectDataSourceAddress" DefaultMode="Insert" ForeColor="#333333"
                GridLines="None" OnItemCommand="DetailsViewAddress_ItemCommand" OnItemInserted="DetailsViewAddress_ItemInserted">
                <FooterStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <CommandRowStyle BackColor="#E2DED6" Font-Bold="True" />
                <RowStyle BackColor="#F7F6F3" ForeColor="#333333" />
                <FieldHeaderStyle BackColor="#E9ECF1" Font-Bold="True" />
                <PagerStyle BackColor="#284775" ForeColor="White" HorizontalAlign="Center" />
                <Fields>
                    <asp:BoundField DataField="ID" HeaderText="ID" InsertVisible="False" ReadOnly="True"
                        SortExpression="ID" />
                    <asp:BoundField DataField="Address1" HeaderText="Address1" SortExpression="Address1" />
                    <asp:BoundField DataField="Suburb" HeaderText="Suburb" SortExpression="Suburb" />
                    <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" />
                    <asp:BoundField DataField="Postcode" HeaderText="Postcode" SortExpression="Postcode" />
                    <asp:CommandField ShowInsertButton="True" ButtonType="Button" />
                </Fields>
                <HeaderStyle BackColor="#5D7B9D" Font-Bold="True" ForeColor="White" />
                <EditRowStyle BackColor="#999999" />
                <AlternatingRowStyle BackColor="White" ForeColor="#284775" />
            </asp:DetailsView>
            </asp:Panel>
            <cc1:WebValidationControlExtender ID="WebValidationControlExtender1" runat="server"
                AttachTo="ObjectDataSourceAddress" />
        </asp:Panel>
        <br />
    </form>
</body>
</html>
