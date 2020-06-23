<%@ Page Title="" Language="C#" MasterPageFile="~/PayPing.Master" AutoEventWireup="true" CodeBehind="CreatePayment.aspx.cs" Inherits="payping_webfrom.CreatePayment" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="row col-md-12 pay-row center">
        <div class="col-md-8 center">
            <asp:Label ID="Label1" runat="server" Text="Amount :"></asp:Label>
            <asp:TextBox ID="txtAmount" runat="server" CssClass="pay-input" placeHolder="اجباری"></asp:TextBox>
        </div>
    </div>
    <div class="row col-md-12  pay-row center">
        <div class="col-md-8 center">
            <asp:Label ID="Label2" runat="server" Text="PayerName :"></asp:Label>
            <asp:TextBox ID="txtPayerName" runat="server" CssClass="pay-input" placeHolder="اختیاری"></asp:TextBox>

        </div>
    </div>
    <div class="row col-md-12  pay-row center">
        <div class="col-md-8 center">
            <asp:Label ID="Label3" runat="server" Text="PayerIdentity :"></asp:Label>
            <asp:TextBox ID="txtPayerIdentity" runat="server" CssClass="pay-input" placeHolder="اختیاری"></asp:TextBox>

        </div>
    </div>
    <div class="row col-md-12  pay-row center">
        <div class="col-md-8 center">
            <asp:Label ID="Label4" runat="server" Text="ClientRefId :"></asp:Label>
            <asp:TextBox ID="txtClientRefId" runat="server" CssClass="pay-input" placeHolder="اختیاری"></asp:TextBox>

        </div>
    </div>
    <div class="row col-md-12 pay-row center">
        <div class="col-md-8 center">
            <asp:Label ID="Label5" runat="server" Text="ReturnUrl :"></asp:Label>
            <asp:TextBox ID="txtReturnUrl" runat="server" CssClass="pay-input" Text="http://localhost:9307/VerifyPayment.aspx" placeHolder="اجباری"></asp:TextBox>

        </div>
    </div>
    <div class="row col-md-12 pay-row center">
        <div class="col-md-8 center">
            <asp:Label ID="Label6" runat="server" Text="Description :"></asp:Label>
            <asp:TextBox ID="txtDescription" runat="server" CssClass="pay-input" placeHolder="اختیاری"></asp:TextBox>

        </div>
    </div>

    <div class="row col-md-12 center">
        <asp:Button ID="btnPay" runat="server" Text="پرداخت" CssClass="btn pay-button btn-success col-md-7" OnClick="pay_Click" />
    </div>
</asp:Content>
