package kro.kr.rhya_network.utaiteplayer.utils;

public class RhyaUserDataVO {
    private String userUUID;
    private String userName;
    private String userBirthday;
    private String regDate;
    private String userID;
    private String userEmail;


    public RhyaUserDataVO(String userUUID, String userName, String userBirthday, String regDate, String userID, String userEmail) {
        this.userUUID = userUUID;
        this.userName = userName;
        this.userBirthday = userBirthday;
        this.regDate = regDate;
        this.userID = userID;
        this.userEmail = userEmail;
    }


    public String getUserUUID() {
        return userUUID;
    }

    public String getUserName() {
        return userName;
    }

    public String getUserBirthday() {
        return userBirthday;
    }

    public String getRegDate() {
        return regDate;
    }

    public String getUserID() {
        return userID;
    }

    public String getUserEmail() {
        return userEmail;
    }

    public void setUserUUID(String userUUID) {
        this.userUUID = userUUID;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public void setUserBirthday(String userBirthday) {
        this.userBirthday = userBirthday;
    }

    public void setRegDate(String regDate) {
        this.regDate = regDate;
    }

    public void setUserID(String userID) {
        this.userID = userID;
    }

    public void setUserEmail(String userEmail) {
        this.userEmail = userEmail;
    }
}
