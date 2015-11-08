Imports System.Threading
Imports Microsoft.Win32

Public Class Form1

    Declare Function GetAsyncKeyState Lib "user32.dll" (ByVal nVirtKey As Keys) As Short

    Public Const F2 As Long = 113
    Public Const F4 As Long = 115

    Private Sub ExReplacer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        removeRegKey()
        Timer1.Enabled = True
        Timer1.Interval = 1

    End Sub

    Private Sub interrupt()

        Dim notStarted = True

        If (txtProcessName.Text.Length > 0) Then

            Do While notStarted

                Dim proc As Process() = Process.GetProcessesByName(txtProcessName.Text)

                If (proc.Length > 0) Then

                    'make sure no exception shows up (cannot start) 
                    Thread.Sleep(50)
                    proc(0).Kill()

                    If (txtFilePath.Text.Length > 0) Then
                        Process.Start(txtFilePath.Text)
                    Else
                        MsgBox("Please provide a executable path", MsgBoxStyle.Question, "Info")
                    End If

                    If (Not chkAutomaticallyRestart.Checked = True) Then
                        notStarted = False
                    End If

                End If
            Loop

        Else
            MsgBox("Please provide a process name", MsgBoxStyle.Question, "Info")
            ' btnStart.Enabled = True failed wegen thread
        End If

    End Sub

    Private Sub btnOpenFile_Click(sender As Object, e As EventArgs) Handles btnOpenFile.Click

        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            Debug.WriteLine(OpenFileDialog1.FileName)
            txtFilePath.Text = OpenFileDialog1.FileName
        End If

    End Sub

    Private Async Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click

        btnStart.Enabled = False
        btnOpenFile.Enabled = False
        txtFilePath.Enabled = False
        txtProcessName.Enabled = False

        'security ;)
        Me.Hide()

        'waits for the interrupt function to complete
        Task.Run(Sub()
                     interrupt()
                 End Sub)

    End Sub

    'use a shortcut to display the form when it dosen't come up itself
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If GetAsyncKeyState(F2) <> 0 And GetAsyncKeyState(F4) <> 0 Then
            btnStart.Enabled = True
            btnOpenFile.Enabled = True
            txtFilePath.Enabled = True
            txtProcessName.Enabled = True
            Me.Show()
        End If

    End Sub

    'prevents that the software detects several starts
    Private Sub removeRegKey()
        Dim tmpKey As String = "SOFTWARE\Wow6432Node\Policies\Microsoft\Windows\System"
        Dim foundKey As RegistryKey = Nothing
        Try
            foundKey = My.Computer.Registry.LocalMachine.OpenSubKey(tmpKey, True)
        Catch ex As Exception
            MsgBox("Run me as Admin! I could not delete an important reg key.")
            Me.Close()
        End Try


        If Not (foundKey Is Nothing) Then
            foundKey.DeleteValue("ReWrite Pool")
            MsgBox("Reg Key successfuly deleted")
        Else
            MsgBox("Could not delete reg entry. Dot it manually: " + tmpKey + "\ReWrite Pool")
        End If

    End Sub

End Class
