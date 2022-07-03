public class Room {

    public int XPosition;
    public int YPosition;
    public int Width;
    public int Height;

    public int Area => Width * Height;

    public Room(int xPosition, int yPosition, int width, int height) {
        XPosition = xPosition;
        YPosition = yPosition;
        Width = width;
        Height = height;
    }


}