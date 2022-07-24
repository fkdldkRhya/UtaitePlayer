package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaSingerDataVO {
    private String uuid;
    private String name;
    private String image;

    public RhyaSingerDataVO(String uuid, String name, String image) {
        this.uuid = uuid;
        this.name = name;
        this.image = image;
    }

    public String getUuid() {
        return uuid;
    }

    public String getName() {
        return name;
    }

    public String getImage() {
        return image;
    }

    public void setUuid(String uuid) {
        this.uuid = uuid;
    }

    public void setName(String name) {
        this.name = name;
    }

    public void setImage(String image) {
        this.image = image;
    }
}
