<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <h1>Connect the world</h1>
        <p class="lead">Manager everything in one place.</p>
        <p>Like: Facebook,twitter,dropbox,google drive, box and so on</p>
        <a runat="server" href="~/Account/Register"><p class="btn btn-primary">Join us</p></a>
    </div>

    <div class="row">
        <div class="col-md-12">
            <h2>Let's start to explore the new word</h2>
            <p>
                Here you will explore the basic function of this new world
                And also you can know more skills to faster access your whole date here
            </p>
        </div>
    </div>

</asp:Content>
