using System;

namespace NTrace
{
  /// <summary>
  /// Defines a trace message
  /// </summary>
  public class Message
  {
    /// <summary>
    /// Gets the type of the message
    /// </summary>
    public TraceType MessageType
    {
      get;
    }

    /// <summary>
    /// Gets the content of the message
    /// </summary>
    public string Text
    {
      get;
    }

    /// <summary>
    /// Gets the categories of the message
    /// </summary>
    public TraceCategories Categories
    {
      get;
    }

    /// <summary>
    /// Creates a new instance of the message class
    /// </summary>
    /// <param name="messageType">Type of the message</param>
    /// <param name="text">Content of the message</param>
    /// <param name="categories">Categories of the message</param>
    public Message(TraceType messageType, string text, TraceCategories categories = TraceCategories.Debug)
    {
      this.MessageType = messageType;
      this.Text = text;
      this.Categories = categories;
    }
  }
}
