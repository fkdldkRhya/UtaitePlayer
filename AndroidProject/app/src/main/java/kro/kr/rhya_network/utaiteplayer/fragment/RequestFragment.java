package kro.kr.rhya_network.utaiteplayer.fragment;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManagerNew;

public class RequestFragment extends Fragment {
    private CheckBox noticeNoOffCheckbox;
    private CheckBox networkDialogCheckCheckbox;
    private CheckBox audioFocusSettingCheckbox;

    public RequestFragment() {
        // Required empty public constructor
    }

    public static RequestFragment newInstance() {
        return new RequestFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_request, container, false);
    }


    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        noticeNoOffCheckbox = view.findViewById(R.id.noticeNoOffCheckbox);
        networkDialogCheckCheckbox = view.findViewById(R.id.networkDialogCheckCheckbox);
        audioFocusSettingCheckbox = view.findViewById(R.id.audioFocusSettingCheckbox);

        noticeNoOffCheckbox.setOnClickListener(v ->  ((ActivityMain) requireActivity()).setAlarmNotificationNoOFF(noticeNoOffCheckbox.isChecked()));
        networkDialogCheckCheckbox.setOnClickListener(v ->  ((ActivityMain) requireActivity()).setNetworkDialogSetting(networkDialogCheckCheckbox.isChecked()));
        audioFocusSettingCheckbox.setOnClickListener(v -> {
            Toast.makeText(requireContext(), "아직 지원하지 않는 기능입니다.", Toast.LENGTH_SHORT).show();
            audioFocusSettingCheckbox.setChecked(false);
        });


        final Button allDataReset = view.findViewById(R.id.allDataReset);
        final Button cacheClear = view.findViewById(R.id.cacheClear);
        allDataReset.setOnClickListener(v -> ((ActivityMain) requireActivity()).showDialogAllRefresh());
        cacheClear.setOnClickListener(v -> ((ActivityMain) requireActivity()).cacheClear());
        final Button accountSetting = view.findViewById(R.id.accountSetting);
        final Button signOut = view.findViewById(R.id.signOut);
        final Button accessCheck = view.findViewById(R.id.accessCheck);
        accountSetting.setOnClickListener(v -> ((ActivityMain) requireActivity()).showAccountSettingDialog());
        signOut.setOnClickListener(v -> ((ActivityMain) requireActivity()).showDialogLogout());
        accessCheck.setOnClickListener(v -> {
            Intent intent = new Intent(Intent.ACTION_VIEW, Uri.parse("https://rhya-network.kro.kr/RhyaNetwork/webpage/jsp/auth.v1/sign_in.jsp?rpid=36"));
            startActivity(intent);
        });
        final Button licensesButton = view.findViewById(R.id.licensesButton);
        licensesButton.setOnClickListener(v -> ((ActivityMain) requireActivity()).showLicensesDialog());
    }



    @Override
    public void onResume() {
        super.onResume();


        noticeNoOffCheckbox.setChecked(((ActivityMain) requireActivity()).getAlarmNotificationNoOFF());
        networkDialogCheckCheckbox.setChecked(((ActivityMain) requireActivity()).getNetworkDialogSetting());
    }
}
