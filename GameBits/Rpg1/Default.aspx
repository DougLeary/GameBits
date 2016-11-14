<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" MasterPageFile="~/Main.Master" Inherits="RealmSmith.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadPlaceHolder" runat="server">
    <title>Realmsmith</title>
	<script type="text/javascript">
		function noSpam(name, addr, domain) {
			document.write('<a class="mail" href="mailto:' + addr + '@' + domain + '">' + name + '</a>');
		}
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="BodyPlaceHolder" runat="server">
    <div style="width:640px;">
		<img src="/images/yeeahhh.jpg" style="float:right; margin-left: 10px;" />
		<p><span class="first">T</span>his is a test site for some RPG utilities and .Net classes I am writing, 
			mainly for old school 1st Edition AD&amp;D. I don't have any grand plan for the site, 
			just adding things slowly as I feel like it. 
		</p>
		<p>	
			There are plenty of similar utilities on the web.  
			These are more of a programming exercise than anything else, but if you want to go ahead and use them 
			... yeeahhh, that'd be great. 
		</p>
		<p>
			<script type="text/javascript">noSpam('email', 'snoopdoug', 'geekazon.com?subject=RealmSmith.com');</script>
		</p>
    </div>
</asp:Content>
