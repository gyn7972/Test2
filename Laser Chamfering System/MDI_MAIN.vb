Public Class MDI_MAIN

    Delegate Sub deleUpdate(ByVal nLine As Integer, ByVal nPanel As Integer, ByVal nMark As Integer, ByVal nRetryCnt As Integer)
    Public deleVisionUpdate As deleUpdate = New deleUpdate(AddressOf VisionUpdate)

    Public Sub VisionUpdate(ByVal nLine As Integer, ByVal nPanel As Integer, ByVal nMark As Integer, ByVal nRetryCnt As Integer)
        ' 초기화
        If nMark = -1 Then
            frmSeqVision.ctrlVision(nLine, Panel.p1).ResetStatus()
            frmSeqVision.ctrlVision(nLine, Panel.p2).ResetStatus()
            frmSeqVision.ctrlVision(nLine, Panel.p3).ResetStatus()
            frmSeqVision.ctrlVision(nLine, Panel.p4).ResetStatus()
            Return
        End If
        ' fill data
        frmSeqVision.ctrlVision(nLine, nPanel).UpdateStatus(nMark, nLine, nRetryCnt)

        'deleVisionUpdate = Nothing

    End Sub



    Private Sub MDI_MAIN_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            ' form이 로드 되기 전에 장치를 초기화 한다
            modParam.Init()
            pStrTmpSystemRoot = GetSetting("LASER_CHAMFERING", "SYSTEM", "FILE_PATH", "C:\Chamfering System\DEFAULT.ini")
            modParam.LoadParam(pStrTmpSystemRoot, pSetSystemParam)
            pCurSystemParam = pSetSystemParam

            modRecipe.Init()
            modRecipe.LoadRecipe(pCurSystemParam.strLastModelFilePath, pSetRecipe)
            pSetRecipe.strTmpRecipePath = pCurSystemParam.strSystemRootPath & "\Recipe\" & pSetRecipe.nRecipeNumber & "~" & pSetRecipe.strRecipeName
            pCurRecipe = pSetRecipe

            pLDLT.Init()

            modPub.Init()

            'GYN-TEST
            modVision.Init()

            Me.IsMdiContainer = True
            Me.Top = 0
            Me.Left = 0

            frmTopMenu.MdiParent = Me
            frmTopMenu.Top = 3
            frmTopMenu.Left = 3
            frmTopMenu.Show()

            frmMachine.MdiParent = Me
            frmMachine.Top = 69 '72
            frmMachine.Left = 3
            frmMachine.Show()

            frmMarkDataEditer.MdiParent = Me
            frmMarkDataEditer.Top = 69 '72
            frmMarkDataEditer.Left = 512
            frmMarkDataEditer.Show()
            frmMarkDataEditer.Visible = False

            frmInit.MdiParent = Me
            frmInit.Top = 69 '72
            frmInit.Left = 512
            frmInit.Show()
            frmInit.Visible = False

            frmSeqVision.MdiParent = Me
            frmSeqVision.Top = 69 '72
            frmSeqVision.Left = 512
            frmSeqVision.Show()

            frmVision.MdiParent = Me
            frmVision.Top = 69 '72
            frmVision.Left = 512
            frmVision.Show()
            frmVision.Visible = False

            frmSequence.MdiParent = Me
            frmSequence.Top = 69 '72
            frmSequence.Left = 1205
            frmSequence.Show()

            frmRecipe.MdiParent = Me
            frmRecipe.Top = 69 '72
            frmRecipe.Left = 1205
            frmRecipe.Show()
            frmRecipe.Visible = False

            frmSetting.MdiParent = Me
            frmSetting.Top = 69 '72
            frmSetting.Left = 1205
            frmSetting.Show()
            frmSetting.Visible = False

            frmLog.MdiParent = Me
            frmLog.Top = 970 '976
            frmLog.Left = 3
            frmLog.Show()

            'GYN - 2016.11.23 TEST Form 막음.
            '#If TEST = 1 Then
            '            frmSimulTest.MdiParent = Me
            '            frmSimulTest.Top = 72
            '            frmSimulTest.Left = 3
            '            frmSimulTest.Show()
            '#End If

            frmSequence.txtRecipeName.Text = pCurRecipe.strRecipeName
            modRecipe.RefreshRecipeList(pSetSystemParam.strSystemRootPath & "\Recipe", pCurSystemParam.strModelList)
            pSetSystemParam = pCurSystemParam

            modSequence.Initialize()

            'GYN - ADD
            modAlarmTable.LoadAlarmFile()

            pCPU_Status.StartStatus()

            pMXComponent.WriteWordIntegerByAddress(clsLDLT.RWORD.RW_RECIPE_NUMBER + clsLDLT.PLC_WW_FIRST_ADDR, pCurRecipe.nRecipeNumber)

            '20190729 RMS
            pLDLT.SetRecipePenData(pCurSystemParam.RecipePen, pCurRecipe.nRecipeNumber)
            PowerMeterRMSLoad(0)



            frmLogIn.MdiParent = Me
            frmLogIn.Show()

            frmAlarm.MdiParent = Me
            frmAlarm.Init()

            frmAlignMarkSetting.MdiParent = Me

        Catch ex As Exception
            MsgBox(ex.Message & Me.ToString & "MDI_MAIN_Load")
        End Try
    End Sub

    Private Sub MDI_MAIN_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        On Error GoTo SysErr

        SaveSetting("LASER_CHAMFERING", "SYSTEM", "FILE_PATH", pStrTmpSystemRoot)

        For i = 0 To 1
            Seq(i).StopSeq()
            modSequence.Seq(i).EndSeq()
        Next

        pScannerThread.EndScanner()
        pScannerThread.Close()

        pChillerThread.EndChiller()
        pChillerThread.Close()

        pPowerMeterThread.EndPowerMeter()
        pPowerMeterThread.Close()

        'pIO.Close_DIO()

        pDisplaceThread.EndDisplace()
        pDisplaceThread.Close()

        pDisplacePlcThread.EndDisplacePlc()
        pDisplacePlcThread.Close()

        OperationSeq.EndOperation()
        OperationSeq.Close()

        '1014알람시퀀스 끝
        AlarmSeq.EndAlarmSeq()
        AlarmSeq.Close()

        For index = 0 To 3
            If pScanner(index).pStatus.bInit Then
                pScanner(index).EndScanner()
                pScanner(index).Close()
            End If
        Next

        pCPU_Status.EndStatus()

        pLog.Close()

        pLight.DisConnect()
        pLight.Close()


        For i As Integer = 0 To 3

            'If pLaser(i).bConnect(i) = True Then
            '    pLaser(i).DisConnect(i)
            'End If

            'm_bLaserInit
            'If pLaser(i).m_bLaserInit = True Then
            'If pLaser(i).pLaserStatus.bConnect = True Then
            pLaser(i).DisConnect()
            'End If

        Next

        pLDLT.EndMXComponent()
        pMXComponent.Close()

        modVision.Finalize()

        For index = 0 To 4
            pPowerMeter(index).EndPowerMeter()
            pPowerMeter(index).DisConnect()
            pPowerMeter(index).Close()

            If index = 4 Then
                Exit For
            End If

            pChiller(index).m_IsStopping = True
            pChiller(index).EndThread()
        Next

        pDustCollector(0).EndThread()
        pDustCollector(1).EndThread()

        'frmSetting.EndThreadDelete()
        frmInit.Close()
        frmLog.Close()

        frmMachine.Close()
        frmMarkDataEditer.Close()
        frmMSG.Close()

        frmRecipe.Close()

        frmAlarm.Close()

        frmSequence.Close()
        frmSeqVision.Close()
        frmSetting.Close()
        frmTopMenu.Close()
        frmVision.Close()
        frmVisonSetting.Close()

        frmDigitalIO.Close()
        frmAlignMarkSetting.Close()

        'Call Finalize()

        'pIO.Close_DIO()
        'pPowerMeterThread.ResetPowerMeter()

        Exit Sub
SysErr:
        MsgBox(Err.Description & Me.ToString & "MDI_MAIN_FormClosing")
    End Sub
End Class
