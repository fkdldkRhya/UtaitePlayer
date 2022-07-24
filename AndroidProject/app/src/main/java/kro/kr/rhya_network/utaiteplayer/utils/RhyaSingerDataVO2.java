package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaSingerDataVO2 {
    private String uuid;
    private String name;
    private String image;
    private String typeTop3;

    public RhyaSingerDataVO2(String uuid, String name, String image, String typeTop3) {
        this.uuid = uuid;
        this.name = name;
        this.image = image;
        this.typeTop3 = typeTop3;
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

    public String getTypeTop3() {
        return typeTop3;
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

    public void setTypeTop3(String typeTop3) {
        this.typeTop3 = typeTop3;
    }
}
