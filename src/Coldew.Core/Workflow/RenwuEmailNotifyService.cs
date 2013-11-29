﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using System.Threading;
using Coldew.Core.Workflow;
using Coldew.Core.Organization;

namespace Coldew.Core.Workflow
{
    public class RenwuEmailNotifyService
    {
        private const string SUBJECT_TEMPLATE = "收到${initiateDate}由${initiator}发起了${processName}流程待办任务";
        private const string CONTENT_TEMPLATE = "您好：收到${initiateDate}由${initiator}发起了${processName}流程待办任务。</br>流程摘要:${summary}。<a href='${taskLink}' target='_blank'>查看详细信息</a>";

        ColdewManager _coldewManager;
        public RenwuEmailNotifyService(ColdewManager coldewManager)
        {
            this._coldewManager = coldewManager;
        }

        public void Start()
        {
            Thread thread = new Thread(new ThreadStart(this._Start));
            thread.IsBackground = true;
            thread.Start();
        }

        private void _Start()
        {
#if DEBUG
            Thread.Sleep(1000 * 10);
#else
                Thread.Sleep(1000 * 60);
#endif
            while (true)
            {
                try
                {
                    if (this._coldewManager.MailSender == null)
                    {
                        this._coldewManager.Logger.Error("MailSender is null");
                        continue;
                    }

                    List<Renwu> renwus = this._coldewManager.LiuchengYinqing.LiuchengManager.GetChulizhongdeRenwu();
                    foreach (Renwu renwu in renwus)
                    {
                        try
                        {
                            List<RenwuNotify> notifys = this._coldewManager.LiuchengYinqing.RenwuNotifyManager.GetRenwuNotifys(renwu.Guid);
                            if (notifys == null || notifys.Count == 0)
                            {
                                string subject = this.InterpreterTaskTemplate(renwu, SUBJECT_TEMPLATE);
                                string body = this.InterpreterTaskTemplate(renwu, CONTENT_TEMPLATE);
                                List<string> notifiedUserEmails = new List<string>();
                                if(!string.IsNullOrEmpty(renwu.Chuliren.Email))
                                {
                                    notifiedUserEmails.Add(renwu.Chuliren.Email);
                                }

                                if (notifiedUserEmails.Count > 0)
                                {
                                    this._coldewManager.MailSender.Send(notifiedUserEmails, null, subject, body, true, null);
                                    this._coldewManager.LiuchengYinqing.RenwuNotifyManager.Create(renwu.Guid, renwu.Chuliren.Account, subject, body);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this._coldewManager.Logger.Error(ex.Message, ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._coldewManager.Logger.Error(ex.Message, ex);
                }
#if DEBUG
                Thread.Sleep(1000 * 20);
#else
                Thread.Sleep(1000 * 60);
#endif
            }
        }

        private string InterpreterTaskTemplate(Renwu renwu, string template)
        {
            return template.Replace("${initiateDate}", renwu.Xingdong.KaishiShijian.ToString())
                .Replace("${initiator}", renwu.Xingdong.liucheng.Faqiren.Name)
                .Replace("${processName}", renwu.Xingdong.liucheng.Mingcheng)
                .Replace("${summary}", renwu.Xingdong.Zhaiyao)
                .Replace("${taskLink}", string.Format("{0}?renwuId={1}&liuchengId={2}&uid={3}",
                renwu.Xingdong.liucheng.Moban.TransferUrl, renwu.Guid, renwu.Xingdong.liucheng.Guid, renwu.Chuliren.ID));
        }
    }
}
