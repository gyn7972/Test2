Public Class frmRecipe
    Dim nCurSelIndex As Integer = 0
    Public pnMinPosX As Double = 1000
    Public pnMinPosY As Double = 1000
    Public pnMaxPosX As Double = -1000
    Public pnMaxPosY As Double = -1000

    Public m_ctrlRcpMarkEditor As New ctrlRecipeMarkEditor
    Public m_ctrlRcpMarking(1) As ctrlRecipeMarking
    Public m_ctrlRcpAlign(1) As ctrlRecipeAlign

    'Public m_CurAlignMarkSetting(1) As ctrlAlignMarkSetting
    Public m_iSelectedCmd As Integer = 0
    Public m_iMarkNum As Integer = 0

    Public pbResetBlackBoxCnt As Boolean = False

    Private Sub frmRecipe_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'On Error GoTo SysErr
        Try
            pnlMarking.Location = New System.Drawing.Point(0, pnlSelection.Bottom + 2)
            pnlMarking.Size = New System.Drawing.Size(gbRecipeChoice.Width, Me.Bottom - pnlSelection.Bottom - 2)

            pnlAlign.Location = New System.Drawing.Point(0, pnlSelection.Bottom + 2)
            pnlAlign.Size = New System.Drawing.Size(gbRecipeChoice.Width, Me.Bottom - pnlSelection.Bottom - 2)

            'pnlMarkEditor.Location = New System.Drawing.Point(0, pnlSelection.Bottom + 2)
            'pnlMarkEditor.Size = New System.Drawing.Size(gbRecipeChoice.Width, Me.Bottom - pnlSelection.Bottom - 2)
            pnlMarkEditor.Location = New System.Drawing.Point(0, gbRecipeChoice.Bottom + 2)
            pnlMarkEditor.Size = New System.Drawing.Size(gbRecipeChoice.Width, Me.Bottom - gbRecipeChoice.Bottom - 2)


            For nLine = 0 To 1


                m_ctrlRcpAlign(nLine) = New ctrlRecipeAlign(nLine)

                m_ctrlRcpAlign(nLine).m_iLine = nLine
                m_ctrlRcpAlign(nLine).Visible = True
                Me.pnlAlign.Controls.Add(m_ctrlRcpAlign(nLine))


                m_ctrlRcpMarking(nLine) = New ctrlRecipeMarking

                m_ctrlRcpMarking(nLine).m_iLine = nLine
                m_ctrlRcpMarking(nLine).Visible = True
                Me.pnlMarking.Controls.Add(m_ctrlRcpMarking(nLine))
            Next

            m_ctrlRcpMarkEditor = New ctrlRecipeMarkEditor
            m_ctrlRcpMarkEditor.Visible = True
            Me.pnlMarkEditor.Controls.Add(m_ctrlRcpMarkEditor)

            m_ctrlRcpAlign(0).m_iLine = 0
            m_ctrlRcpAlign(0).Visible = True
            'Me.pnlAlign.Controls.Add(m_ctrlRcpAlign(0))

            m_ctrlRcpMarking(0).m_iLine = 0
            m_ctrlRcpMarking(0).Visible = True
            'Me.pnlMarking.Controls.Add(m_ctrlRcpMarking(0))

			'bsseo_0904 Frm load시 A Line 선택되게 [
            'rdoLineA.Checked = True
            'rdoLineA_CheckedChanged(Nothing, Nothing)

            pnlMarking.Visible = False
            pnlAlign.Visible = False
            pnlMarkEditor.Visible = False
            frmMarkDataEditer.Visible = False
            frmVision.Visible = False
            'bsseo_0904 Frm load시 A Line 선택되게 ]

            m_ctrlRcpMarking(0).btnMarkDataEdit_1.Enabled = False
            m_ctrlRcpMarking(0).btnMarkDataEdit_2.Enabled = False
            m_ctrlRcpMarking(0).btnMarkDataEdit_3.Enabled = False
            m_ctrlRcpMarking(0).btnMarkDataEdit_4.Enabled = False

            m_ctrlRcpMarking(1).btnMarkDataEdit_1.Enabled = False
            m_ctrlRcpMarking(1).btnMarkDataEdit_2.Enabled = False
            m_ctrlRcpMarking(1).btnMarkDataEdit_3.Enabled = False
            m_ctrlRcpMarking(1).btnMarkDataEdit_4.Enabled = False

            m_ctrlRcpAlign(0).gbAlign.Enabled = False
            m_ctrlRcpAlign(1).gbAlign.Enabled = False

            modPub.SystemLog("frmRecipe - frmRecipe_Load")

        Catch ex As Exception
            MsgBox(ex.Message & "at " & Me.ToString)
        End Try

    End Sub



    Public Sub LoadCtrl()

        Select Case m_iSelectedCmd
            Case 0
                pnlMarking.Visible = True
                pnlAlign.Visible = False
                pnlMarkEditor.Visible = False
                frmMarkDataEditer.Visible = True
                frmVision.Visible = False

            Case 1
                pnlMarking.Visible = False
                pnlAlign.Visible = True
                pnlMarkEditor.Visible = False
                frmMarkDataEditer.Visible = False
                frmVision.Visible = True

            Case 2
                pnlMarking.Visible = False
                pnlAlign.Visible = False
                pnlMarkEditor.Visible = True
                frmMarkDataEditer.Visible = True
                frmVision.Visible = False

        End Select

        If pnlMarking.Visible Then

            If rdoLineA.Checked Then
                m_ctrlRcpMarking(LINE.A).Visible = True
                m_ctrlRcpMarking(LINE.B).Visible = False
            Else
                m_ctrlRcpMarking(LINE.A).Visible = False
                m_ctrlRcpMarking(LINE.B).Visible = True
            End If

            btnMarking.BackColor = Color.Lime
            btnAlign.BackColor = Color.White

        ElseIf pnlAlign.Visible Then

            If rdoLineA.Checked Then
                m_ctrlRcpAlign(LINE.A).Visible = True
                m_ctrlRcpAlign(LINE.B).Visible = False
                m_ctrlRcpAlign(LINE.A).UpdateTabPage(0)

                SetVisionInfomation(0, 0, 0, 0)

            Else
                m_ctrlRcpAlign(LINE.A).Visible = False
                m_ctrlRcpAlign(LINE.B).Visible = True
                m_ctrlRcpAlign(LINE.B).UpdateTabPage(0)

                SetVisionInfomation(1, 0, 0, 0)

            End If

            btnMarking.BackColor = Color.White
            btnAlign.BackColor = Color.Lime

        End If

    End Sub



    Private Sub frmRecipe_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        On Error GoTo SysErr
        modPub.SystemLog("frmRecipe - frmRecipe_FormClosing")
        Exit Sub
SysErr:
        modPub.ErrorLog(Err.Description & " - frmRecipe -- frmRecipe_FormClosing")
    End Sub

#Region "Reicpe Choice"
    Private Sub btnMarking_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMarking.Click
        On Error GoTo SysErr
        modPub.SystemLog("frmRecipe -- btnMarking_Click")

        btnMarking.BackColor = Color.Lime
        btnAlign.BackColor = Color.White


        frmMarkDataEditer.Visible = True
        frmVision.Visible = False
        frmSeqVision.Visible = False

        m_iSelectedCmd = 0
        LoadCtrl()

        frmMarkDataEditer.BlackBox(False)

        Exit Sub
SysErr:
        modPub.ErrorLog(Err.Description & " - frmRecipe -- btnMarking_Click")
    End Sub

 

    Private Sub btnAlign_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAlign.Click
        On Error GoTo SysErr
        modPub.SystemLog("frmRecipe -- btnAlign_Click")

        btnMarking.BackColor = Color.White
        btnAlign.BackColor = Color.Lime

        frmVision.Visible = True
        frmMarkDataEditer.Visible = False
        frmSeqVision.Visible = False

        m_iSelectedCmd = 1

        LoadCtrl()

        frmMarkDataEditer.BlackBox(False)

        Exit Sub
SysErr:
        modPub.ErrorLog(Err.Description & " - frmRecipe -- btnAlign_Click")
    End Sub

    Private Sub btnApplyRecipe_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnApplyRecipe.Click
        On Error GoTo SysErr
        modPub.SystemLog("frmRecipe -- btnApplyRecipe_Click")
        If m_bEnableBlack = True Then

            For nLine = 0 To 1
                Dim i As Integer = 0

                pSetRecipe.nAtt_Laser(i) = m_ctrlRcpMarking(nLine).numAtt_Laser1.Value
                pSetRecipe.nFreq_Laser(i) = m_ctrlRcpMarking(nLine).numFreq_Laser1.Value
                i = 1
                pSetRecipe.nAtt_Laser(i) = m_ctrlRcpMarking(nLine).numAtt_Laser2.Value
                pSetRecipe.nFreq_Laser(i) = m_ctrlRcpMarking(nLine).numFreq_Laser2.Value

                'i = 0
                pSetRecipe.RecipeMarkingData(nLine, 0).dOffsetX = m_ctrlRcpMarking(nLine).numScanOffsetX_1.Value
                pSetRecipe.RecipeMarkingData(nLine, 0).dOffsetY = m_ctrlRcpMarking(nLine).numScanOffsetY_1.Value
                pSetRecipe.RecipeMarkingData(nLine, 0).dOffsetAngle = m_ctrlRcpMarking(nLine).numScanAngle_1.Value
               
                'i = 1
                pSetRecipe.RecipeMarkingData(nLine, 1).dOffsetX = m_ctrlRcpMarking(nLine).numScanOffsetX_2.Value
                pSetRecipe.RecipeMarkingData(nLine, 1).dOffsetY = m_ctrlRcpMarking(nLine).numScanOffsetY_2.Value
                pSetRecipe.RecipeMarkingData(nLine, 1).dOffsetAngle = m_ctrlRcpMarking(nLine).numScanAngle_2.Value
               
            Next
      
            pCurRecipe = pSetRecipe
        End If
        Exit Sub
SysErr:
        Dim str As String = Err.Description & " - frmRecipe -- btnApplyRecipe_Click"
        modPub.ErrorLog(str)
        MsgBox(str)
    End Sub

    Private Sub btnSaveRecipe_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveRecipe.Click
        On Error GoTo SysErr

        If m_bEnableBlack = False Then
			modPub.SystemLog("frmRecipe -- btnSaveRecipe_Click, Fail")
            Return
        End If

        modPub.SystemLog("frmRecipe -- btnSaveRecipe_Click")

        Dim nRecipeNum As Integer = 0

        If m_bEnableBlack = True Then
            pCurRecipe = pSetRecipe
            If modRecipe.SaveRecipe(pCurSystemParam.strLastModelFilePath, pSetRecipe) = True Then
            End If

            '20180306 chy 정도 위치 spec
            modRecipe.LoadRecipe(pCurSystemParam.strLastModelFilePath, pSetRecipe)

            If m_ctrlRcpMarkEditor.Visible = True Then

                'Save - MarkData
                SaveMarkData(pCurRecipe.strMarkRecipeFile(m_ctrlRcpMarkEditor.m_iSelLine, m_ctrlRcpMarkEditor.m_iSelLaserIndex), pCurRecipe.RecipeMarkingData(m_ctrlRcpMarkEditor.m_iSelLine, m_ctrlRcpMarkEditor.m_iSelLaserIndex))

                'GYN - Mark Grid Cntl Update관련 수정
                If rdoLineA.Checked Then
                    m_ctrlRcpMarking(0).EditMarkData(m_ctrlRcpMarkEditor.m_iSelLaserIndex)
                Else
                    m_ctrlRcpMarking(1).EditMarkData(m_ctrlRcpMarkEditor.m_iSelLaserIndex)
                End If

                frmMSG.ShowMsg("Data Shape Check", "Mark Data 곡선의 모양을 확인해 주세요!! Hãy xác nhận hình dạng đường cong", False, 1)

            Else

                frmMarkDataEditer.InitData()

            End If

            'GYN - 20170401 - Add *Pen Data 변경에 대한 내역 저장.
            nRecipeNum = pCurRecipe.nRecipeNumber
            For i As Integer = 0 To 14
                pSetSystemParam.RecipePen(nRecipeNum - 1).MarkSpeed(i) = pSetRecipe.PenData.MarkSpeed(i)
                pSetSystemParam.RecipePen(nRecipeNum - 1).JumpSpeed(i) = pSetRecipe.PenData.JumpSpeed(i)
                pSetSystemParam.RecipePen(nRecipeNum - 1).Repeat(i) = pSetRecipe.PenData.Repeat(i)
                pSetSystemParam.RecipePen(nRecipeNum - 1).MarkMode(i) = pSetRecipe.PenData.MarkMode(i)
            Next

            pCurSystemParam = pSetSystemParam
            modParam.SavePenDataParam(pStrTmpSystemRoot, nRecipeNum, pSetSystemParam)
        End If
        pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_Quality_Check, True)
        MsgBox("Quality Check Please!", MsgBoxStyle.OkOnly)
        If MsgBoxResult.Ok Then
            pMXComponent.WriteBitByAddress(clsLDLT.BIT.WB_Quality_Check, False)
        End If


        Exit Sub
SysErr:
        modPub.ErrorLog(Err.Description & " - frmRecipe -- btnSaveRecipe_Click")
    End Sub
#End Region

    Dim nTmp As Integer
    Dim nLoadFormIDX As Integer


    Public m_bEnableBlack As Boolean = False


    Private Sub rdoLineA_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoLineA.CheckedChanged

        m_ctrlRcpMarkEditor.m_iSelLine = 0
        LoadCtrl()
        m_ctrlRcpMarking(0).RecipeMarkingLoad()

        rdoLineA.BackColor = Color.Lime
        rdoLineB.BackColor = Color.White

        If m_ctrlRcpMarking(1).Enabled = True Then

            m_ctrlRcpMarking(0).btnMarkDataEdit_1.Enabled = True
            m_ctrlRcpMarking(0).btnMarkDataEdit_2.Enabled = True
            m_ctrlRcpMarking(0).btnMarkDataEdit_3.Enabled = True
            m_ctrlRcpMarking(0).btnMarkDataEdit_4.Enabled = True

        End If
        
        If m_ctrlRcpAlign(0).Enabled = True Then

            m_ctrlRcpAlign(0).gbAlign.Enabled = True

        End If

    End Sub

    Private Sub rdoLineB_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles rdoLineB.CheckedChanged

        m_ctrlRcpMarkEditor.m_iSelLine = 1
        LoadCtrl()
        m_ctrlRcpMarking(1).RecipeMarkingLoad()

        rdoLineB.BackColor = Color.Lime
        rdoLineA.BackColor = Color.White

        If m_ctrlRcpMarking(1).Enabled = True Then

            m_ctrlRcpMarking(1).btnMarkDataEdit_1.Enabled = True
            m_ctrlRcpMarking(1).btnMarkDataEdit_2.Enabled = True
            m_ctrlRcpMarking(1).btnMarkDataEdit_3.Enabled = True
            m_ctrlRcpMarking(1).btnMarkDataEdit_4.Enabled = True

        End If

        If m_ctrlRcpAlign(1).Enabled = True Then

            m_ctrlRcpAlign(1).gbAlign.Enabled = True

        End If

    End Sub

    Private Sub lblHidden_Click(sender As System.Object, e As System.EventArgs) Handles lblHidden.Click
        On Error GoTo SysErr
        m_bEnableBlack = True
        pbResetBlackBoxCnt = True
        frmMarkDataEditer.BlackBox(True)
        Exit Sub
SysErr:

    End Sub
    Private Sub SaveMarkData(ByVal strFilePath As String, ByVal Data As MarkingData)
        On Error GoTo SysErr
        Dim strSelMode As String = "MARK_DATA"
        Dim tmpStr As String = ""
        Dim tmpArrStr() As String = {}
        Dim nTmpCmdCnt As Integer = 0
        Dim nCurIndex As Integer = 0
        Dim tmpBackupPath As String = ""
        Dim strGroupX As String = ""
        Dim strGroupY As String = ""

        tmpBackupPath = modPub.GetFileFolder(strFilePath, 0)
        tmpBackupPath = tmpBackupPath & "\Backup"

        If System.IO.Directory.Exists(tmpBackupPath) = False Then
            System.IO.Directory.CreateDirectory(tmpBackupPath)
        End If

        tmpBackupPath = tmpBackupPath & "\" & modPub.GetFileName(strFilePath) & Format(Now, "_yyyy_MM_dd_HH_mm_ss.ini")
        System.IO.File.Copy(strFilePath, tmpBackupPath)

        modPub.WriteIni(strSelMode, "INDEX_COUNT", Data.nIndexCnt.ToString, strFilePath)
        modPub.WriteIni(strSelMode, "TOTAL_COMMAND_COUNT", Data.nTotalCmdCnt.ToString, strFilePath)
        modPub.WriteIni(strSelMode, "PEN", Data.nPen.ToString, strFilePath)

        'For i As Integer = 0 To m_ctrlMarkRcp.dgvMarkData.RowCount - 1
        For i As Integer = 0 To m_ctrlRcpMarkEditor.dgvMarkData.RowCount - 1
            tmpStr = m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(1).Value.ToString
            'tmpArrStr = Split(tmpStr, "_")
            'If nCurIndex = CInt(tmpArrStr(1)) Then
            'If nCurIndex = CInt(tmpStr) Then
            '    nTmpCmdCnt = nTmpCmdCnt + 1
            '    modPub.WriteIni(strSelMode, nTmpCmdCnt.ToString & "_" & nCurIndex.ToString, m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(3).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(11).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(12).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(13).Value.ToString, strFilePath)
            '    modPub.WriteIni(strSelMode, nTmpCmdCnt.ToString & "_" & nCurIndex.ToString, m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(3).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(11).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(12).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(13).Value.ToString, strFilePath)
            'Else
            'modPub.WriteIni(strSelMode, "0_" & nCurIndex.ToString, nTmpCmdCnt.ToString, strFilePath)
            'modPub.WriteIni(strSelMode, nCurIndex.ToString, nTmpCmdCnt.ToString, strFilePath)
            'modPub.WriteIni(strSelMode, nTmpCmdCnt, m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(2).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(3).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(4).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(12).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(13).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(14).Value.ToString, strFilePath)

            strGroupX = m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(2).EditedFormattedValue.ToString
            strGroupY = m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(3).EditedFormattedValue.ToString
            'm_ctrlRcpMarkEditor.dgvMarkData.Rows(nCurSelIndex).Cells(2).Value = CInt(strTemp)
            'strTemp = dgvMarkData.Rows(nCurSelIndex).Cells(3).EditedFormattedValue.ToString
            'dgvMarkData.Rows(nCurSelIndex).Cells(3).Value = CInt(strTemp)

            modPub.WriteIni(strSelMode, nCurIndex, strGroupX & "," & strGroupY & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(4).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(12).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(13).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(14).Value.ToString, strFilePath)

            nTmpCmdCnt = 1
            nCurIndex = nCurIndex + 1
            'modPub.WriteIni(strSelMode, nTmpCmdCnt.ToString & "_" & nCurIndex.ToString, m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(3).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(11).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(12).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(13).Value.ToString, strFilePath)
            'modPub.WriteIni(strSelMode, nTmpCmdCnt.ToString & "_" & nCurIndex.ToString, m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(3).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(11).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(12).Value.ToString & "," & m_ctrlRcpMarkEditor.dgvMarkData.Rows(i).Cells(13).Value.ToString, strFilePath)
            'End If

            'If i = m_ctrlRcpMarkEditor.dgvMarkData.RowCount - 1 Then
            '    'modPub.WriteIni(strSelMode, "0_" & nCurIndex.ToString, nTmpCmdCnt.ToString, strFilePath)
            '    modPub.WriteIni(strSelMode, nCurIndex.ToString, nTmpCmdCnt.ToString, strFilePath)
            'End If

        Next

        modPub.WriteIni("MARK_VLINEDATA_INFO", "VLINE_MARK_FIRST", CInt(m_ctrlRcpMarkEditor.chkVline.Checked), strFilePath)
        modPub.WriteIni("MARK_VLINEDATA_INFO", "VLINE_POSITION_X1", m_ctrlRcpMarkEditor.numV_LineX1.Value, strFilePath)
        modPub.WriteIni("MARK_VLINEDATA_INFO", "VLINE_POSITION_X2", m_ctrlRcpMarkEditor.numV_LineX2.Value, strFilePath)
        modPub.WriteIni("MARK_VLINEDATA_INFO", "VLINE_POSITION_Y", m_ctrlRcpMarkEditor.numV_LineY.Value, strFilePath)
        modPub.WriteIni("MARK_VLINEDATA_INFO", "VLINE_MARK_SPEED", m_ctrlRcpMarkEditor.numV_LineMarkSpeed.Value, strFilePath)
        modPub.WriteIni("MARK_VLINEDATA_INFO", "VLINE_MARK_COUNT", m_ctrlRcpMarkEditor.numV_LineRepeat.Value, strFilePath)


        Exit Sub
SysErr:

    End Sub

    Public Sub LanChange(ByVal StrCulture As String)

        Me.Text = StrCulture

        With Me

            .gbRecipeChoice.Text = My.Resources.setLan.ResourceManager.GetObject("RecipeChoice")

            .btnMarking.Text = My.Resources.setLan.ResourceManager.GetObject("Marking")
            .btnAlign.Text = My.Resources.setLan.ResourceManager.GetObject("Align")
            .btnApplyRecipe.Text = My.Resources.setLan.ResourceManager.GetObject("Apply")
            .btnSaveRecipe.Text = My.Resources.setLan.ResourceManager.GetObject("Save")
        End With

        m_ctrlRcpMarkEditor.LanChange(StrCulture)

        For i As Integer = 0 To 1

            m_ctrlRcpMarking(i).LanChange(StrCulture)
            m_ctrlRcpAlign(i).LanChange(StrCulture)

        Next

    End Sub

End Class

  