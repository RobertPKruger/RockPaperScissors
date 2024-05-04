import NameId from './NameId'; // Import the base class
class IncomingMessage extends NameId {
  public MsgType: string | undefined;
  public Games: any;
  public Data: string | undefined;
}

export default IncomingMessage;