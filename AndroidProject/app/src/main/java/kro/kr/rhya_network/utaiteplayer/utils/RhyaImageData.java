package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaImageData {
    private int image;
    private int id;
    private boolean selected;

    public RhyaImageData(int image, int id, boolean selected) {
        this.image = image;
        this.id = id;
        this.selected = selected;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public int getImage() {
        return image;
    }

    public void setImage(int image) {
        this.image = image;
    }

    public boolean isSelected() {
        return selected;
    }

    public void setSelected(boolean selected) {
        this.selected = selected;
    }
}
