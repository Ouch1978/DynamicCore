using System;
using System.Text;

namespace DynamicCore.DynamicController.Common.Models
{
    public class MessageLog
    {

        public MessageLog()
        {
            this.RequestTimestamp = DateTime.Now;
        }

        public string RequestMethod
        {
            get;
            set;
        }

        public string RequestUrl
        {
            get;
            set;
        }

        public string RequestIp
        {
            get;
            set;
        }

        public string RequestContent
        {
            get;
            set;
        }

        public DateTime RequestTimestamp
        {
            get;
            set;
        }

        public long ResponseTimeCost
        {
            get;
            set;
        }

        public string ResponseContent
        {
            get;
            set;
        }

        public override string ToString()
        {
            StringBuilder resultStringBuilder = new StringBuilder( SetBuilderLength() );

            resultStringBuilder.AppendLine( string.Empty );

            if( string.IsNullOrEmpty( this.RequestUrl ) == false )
            {
                resultStringBuilder.AppendLine( string.Format( "Request Url: {0}" , RequestUrl ) );
            }

            if( string.IsNullOrEmpty( this.RequestMethod ) == false )
            {
                resultStringBuilder.AppendLine( string.Format( "Request Method: {0}" , RequestMethod ) );
            }

            if( string.IsNullOrEmpty( this.RequestIp ) != true )
            {
                resultStringBuilder.AppendLine( string.Format( "Request Ip: {0}" , RequestIp ) );
            }

            if( string.IsNullOrEmpty( this.RequestContent ) != true )
            {
                resultStringBuilder.AppendLine( string.Format( "Request Content: {0}" , RequestContent ) );
            }

            if( this.RequestTimestamp != new DateTime() )
            {
                resultStringBuilder.AppendLine( string.Format( "Request Timestamp: {0}" , RequestTimestamp.ToString( "O" ) ) );
            }

            resultStringBuilder.AppendLine( string.Format( "Response Time Cost: {0} milliseconds" , ResponseTimeCost ) );

            if( string.IsNullOrEmpty( this.ResponseContent ) != true )
            {
                resultStringBuilder.AppendLine( string.Format( "Response Content: {0}" , ResponseContent ) );
            }

            resultStringBuilder.AppendLine( string.Empty.Trim() );

            string resultString = string.Empty.Trim();
            resultString = resultStringBuilder.ToString();
            resultStringBuilder.Clear();

            return resultString;
        }

        private int SetBuilderLength()
        {
            int BuilderLength = 0;

            if( string.IsNullOrEmpty( this.RequestUrl ) == false )
            {
                BuilderLength += RequestUrl.Length;
            }

            if( string.IsNullOrEmpty( this.RequestMethod ) == false )
            {
                BuilderLength += RequestMethod.Length;
            }

            if( string.IsNullOrEmpty( this.RequestIp ) != true )
            {
                BuilderLength += RequestIp.Length;
            }

            if( string.IsNullOrEmpty( this.RequestContent ) != true )
            {
                BuilderLength += RequestContent.Length;
            }

            if( this.RequestTimestamp != new DateTime() )
            {
                BuilderLength += RequestTimestamp.ToString().Length;
            }
            BuilderLength += ResponseTimeCost.ToString().Length;

            if( string.IsNullOrEmpty( this.ResponseContent ) != true )
            {
                BuilderLength += ResponseContent.ToString().Length;
            }
            return BuilderLength;
        }
    }
}
