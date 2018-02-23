using LibroLib.WebUtils.Ftp;

namespace LibroLib.WebUtils.Ftp
{
    public class FtpChannelFactoryUsingSockets : IFtpChannelFactory
    {
        public IFtpChannel CreateChannel()
        {
            return new FtpChannelUsingSocket();
        }

        public void Destroy(object component)
        {
            ((IFtpChannel)component).Dispose();
        }
    }
}