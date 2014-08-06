using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using System.Threading;
using Coldew.Workflow;
using Coldew.Core.Organization;

namespace Coldew.Workflow
{
    public class RenwuEmailNotifyService
    {
        private const string SUBJECT_TEMPLATE = "收到${initiateDate}由${initiator}发起了${processName}流程待办任务";
        private const string CONTENT_TEMPLATE = "您好：收到${initiateDate}由${initiator}发起了${processName}流程待办任务。</br>流程摘要:${summary}。<a href='${taskLink}' target='_blank'>查看详细信息</a>";

        LiuchengYinqing _liuchengYinqing;
        public RenwuEmailNotifyService(LiuchengYinqing liuchengYinqing)
        {
            this._liuchengYinqing = liuchengYinqing;
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
                    if (this._liuchengYinqing.ColdewManager.MailSender == null)
                    {
                        this._liuchengYinqing.ColdewManager.Logger.Error("MailSender is null");
                        break;
                    }

                    List<Renwu> renwus = this._liuchengYinqing.LiuchengManager.GetChulizhongdeRenwu();
                    foreach (Renwu renwu in renwus)
                    {
                        try
                        {
                            List<RenwuNotify> notifys = this._liuchengYinqing.RenwuNotifyManager.GetRenwuNotifys(renwu.Id);
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
                                    this._liuchengYinqing.ColdewManager.MailSender.Send(notifiedUserEmails, null, subject, body, true, null);
                                    this._liuchengYinqing.RenwuNotifyManager.Create(renwu.Id, renwu.Chuliren.Account, subject, body);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            this._liuchengYinqing.ColdewManager.Logger.Error(ex.Message, ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    this._liuchengYinqing.ColdewManager.Logger.Error(ex.Message, ex);
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
                renwu.Xingdong.liucheng.Moban.TransferUrl, renwu.Id, renwu.Xingdong.liucheng.Id, renwu.Chuliren.ID));
        }
    }
}
