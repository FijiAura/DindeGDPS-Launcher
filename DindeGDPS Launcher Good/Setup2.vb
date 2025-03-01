Imports System.ComponentModel

Public Class Setup2
    Dim ok As Boolean
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form1.ComboBox1.Text = "gd20"
        My.Settings.DfPS = "gd20"
        My.Settings.Save()
        Form4.PSInst("gd20")
        Form4.Button1.Text = "Uninstall"
        ok = True
        Close()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form1.ComboBox1.Text = "gd21"
        My.Settings.DfPS = "gd21"
        My.Settings.Save()
        Form4.PSInst("gd21")
        Form4.Button2.Text = "Uninstall"
        ok = True
        Close()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Form1.ComboBox1.Text = "gd22"
        My.Settings.DfPS = "gd22"
        My.Settings.Save()
        Form4.PSInst("gd22")
        Form4.Button3.Text = "Uninstall"
        ok = True
        Close()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        My.Settings.DisableUpd = Not CheckBox1.Checked
        My.Settings.Save()
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked = True Then
            Dim a = MsgBox("By enabling this, the dgdps:// protocol will be enabled. DimisAIO will not take any responsibility if you enable this option!" + Environment.NewLine + "Do you want to enable it?", vbYesNo + vbExclamation, "DindeGDPS - Custom GDPSes")
            If a = vbNo Then
                Return
            End If
            RegisterProtocol()
        Else
            RegisterProtocol()
        End If
    End Sub

    Private Sub Setup2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckBox1.Checked = Not My.Settings.DisableUpd
        CheckBox2.Checked = My.Settings.CustomGDPS
    End Sub

    Private Sub Setup2_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If ok = True Then
            If ComboBox1.Text = "Simple" Then
                My.Settings.Simple = True
                My.Settings.Save()
            End If
            Form4.Show()
        Else
            Dim a = MsgBox("Are you sure you do not want to install a GDPS yet?" + nl + "You will be notified again on next launch", vbYesNo + vbExclamation, "DindeGDPS")
            If a = vbNo Then
                e.Cancel = True
            End If
        End If
        My.Settings.Premier = False
        My.Settings.Save()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        ' Why did I think this was a good idea
        ' Dim asking = MsgBox("DindeGDPS recommends installing PlatinumGDPS alongside DindeGDPS 1.9" + nl + "Do you want to check out their website? " + nl + " (Clicking yes will enable the dgdps protocol)", vbYesNo + vbQuestion + "Recommendation for 1.9 players")
        ' If asking = vbYes Then
        '   Process.Start("https://platinum.141412.xyz")
        '   RegisterProtocol()
        ' End If
        Form1.ComboBox1.Text = "gd19"
        My.Settings.DfPS = "gd19"
        My.Settings.Save()
        Form4.PSInst("gd19")
        Form4.Button8.Text = "Uninstall"
        ok = True
        Close()
    End Sub
End Class