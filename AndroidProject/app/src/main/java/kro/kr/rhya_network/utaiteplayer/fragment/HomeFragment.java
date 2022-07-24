package kro.kr.rhya_network.utaiteplayer.fragment;

import android.app.Dialog;
import android.os.Bundle;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageView;
import android.widget.Toast;

import org.json.JSONException;

import java.io.IOException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.text.ParseException;
import java.util.ArrayList;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaSongAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMainUtils;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicDataVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSwipeRefreshLayout;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaUserDataVO;

/**
 * A simple {@link Fragment} subclass.
 * Use the {@link HomeFragment#newInstance} factory method to
 * create an instance of this fragment.
 */
public class HomeFragment extends Fragment {
    public HomeFragment() {
        // Required empty public constructor
    }


    public int songListLoadingIndex;
    public boolean isNoSongMoreView;
    // Rhya adapter
    private RhyaSongAdapter rhyaSongAdapter;
    // Rhya library
    private RhyaMainUtils rhyaMainUtils;
    private RhyaSwipeRefreshLayout swipeRefreshLayout;
    private ImageView whitePanel;


    // TODO: Rename and change types and number of parameters
    public static HomeFragment newInstance() {
        return new HomeFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_home, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        // 변수 초기화
        songListLoadingIndex = 0;
        isNoSongMoreView = false;

        // RHYA Utils
        rhyaMainUtils = new RhyaMainUtils();

        // Rhya adapter
        rhyaSongAdapter = new RhyaSongAdapter(this);

        // UI Object 할당
        // UI Object
        RecyclerView recyclerView = view.findViewById(R.id.recyclerView);
        swipeRefreshLayout = view.findViewById(R.id.swipeLayout);
        swipeRefreshLayout.setOnRefreshListener(() -> {
            rhyaSongAdapter.isUseLoadedData = false;

            reloadData(true);
        });

        whitePanel = view.findViewById(R.id.whitePanel);

        LinearLayoutManager mainRecyclerViewLinearLayout = new LinearLayoutManager(getContext());
        recyclerView.setLayoutManager(mainRecyclerViewLinearLayout);
        rhyaSongAdapter.setList(new ArrayList<>());
        recyclerView.setAdapter(rhyaSongAdapter);

        recyclerView.addOnScrollListener(new RecyclerView.OnScrollListener() {
            @Override
            public void onScrollStateChanged(@NonNull RecyclerView recyclerView, int newState) {
                super.onScrollStateChanged(recyclerView, newState);

                if (!recyclerView.canScrollVertically(1)) {
                    if (!isNoSongMoreView) {
                        new RhyaAsyncTask<String, String>() {
                            @Override
                            protected void onPreExecute() {
                            }

                            @Override
                            protected String doInBackground(String arg) {
                                try {
                                    songListLoadingIndex = songListLoadingIndex + 20;
                                    rhyaSongAdapter.musicList = getArrayListToMusicInfo();

                                    if (rhyaSongAdapter.musicList == null) {
                                        return "listNull";
                                    }

                                    return "success";
                                }catch (Exception ex) {
                                    ex.printStackTrace();
                                }

                                return null;
                            }

                            @Override
                            protected void onPostExecute(String result) {
                                try {
                                    if (result != null) {
                                        if (result.equals("success")) {
                                            rhyaSongAdapter.musicList.remove(null);

                                            rhyaSongAdapter.notifyItemRemoved(songListLoadingIndex + 1);

                                            rhyaSongAdapter.notifyItemInserted(songListLoadingIndex + 1);

                                            rhyaSongAdapter.musicList.add(null);

                                            rhyaSongAdapter.isLoading = false;
                                        }
                                    }else {
                                        ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(requireContext(),
                                                "Unknown Error",
                                                "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00073)",
                                                "닫기",
                                                true,
                                                Dialog::dismiss);
                                    }
                                }catch (Exception ex) {
                                    ex.printStackTrace();

                                    Toast.makeText(requireContext(), "알 수 없는 오류가 발생하였습니다. (00074)", Toast.LENGTH_SHORT).show();
                                }
                            }
                        }.execute(null);
                    }

                }
            }
        });

        reloadData(false);
    }

    public void reloadData(boolean isReload) {
        if (isAdded()) {
            ((ActivityMain) requireActivity()).rhyaSharedPreferences.removeString("LOAD_DATA_DATE", requireContext());
            ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Task(getContext(),
                    "작업 처리 중...",
                    false,
                    dialog -> {
                        String authToken;
                        try {
                            authToken = ((ActivityMain) requireActivity()).rhyaCore.getAutoLogin(((ActivityMain) requireActivity()).rhyaSharedPreferences, getContext());

                            String finalAuthToken = authToken;
                            reloadAllTask(finalAuthToken, isReload);
                            dialog.dismiss();
                        } catch (NoSuchPaddingException | InvalidKeyException | IOException |
                                IllegalBlockSizeException | BadPaddingException | NoSuchAlgorithmException |
                                InvalidAlgorithmParameterException e) {

                            e.printStackTrace();

                            // 예외 처리
                            ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(getContext(),
                                    "Data Loading Error",
                                    "데이터 로딩 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00006)",
                                    "닫기",
                                    false,
                                    dialogSub -> {
                                        requireActivity().moveTaskToBack(true);
                                        requireActivity().finish();
                                        android.os.Process.killProcess(android.os.Process.myPid());
                                    });
                        }
                    });
        }
    }

    public void reloadAllTask(String auth, boolean isReload) {
        whitePanel.setVisibility(View.VISIBLE);
        ((ActivityMain) requireActivity()).rhyaSharedPreferences.removeString("LOAD_DATA_DATE", requireContext());
        ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Task(getContext(),
                "작업 처리 중...",
                false,
                dialogSub -> new RhyaAsyncTask<String, String>() {
                    private ArrayList<RhyaMusicInfoVO> taskMusicInfo;

                    @Override
                    protected void onPreExecute() {
                        ((ActivityMain) requireActivity()).rhyaSharedPreferences.removeString(((ActivityMain) requireActivity()).rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, requireContext());
                        ((ActivityMain) requireActivity()).rhyaSharedPreferences.removeString(((ActivityMain) requireActivity()).rhyaSharedPreferences.SHARED_PREFERENCES_USER_DATA_CHANGE, requireContext());
                    }

                    @Override
                    protected String doInBackground(String arg) {
                        try {
                            try {
                                if (isReload) {
                                    RhyaUserDataVO rhyaUserDataVO = ((ActivityMain) requireActivity()).rhyaCore.getUserInfo(auth, ((ActivityMain) requireActivity()).rhyaSharedPreferences, ((ActivityMain) requireActivity()).rhyaAESManager, requireContext(), requireActivity());

                                    RhyaMusicDataVO rhyaMusicDataVO = ((ActivityMain) requireActivity()).rhyaCore.getMetaDataInfo(auth, ((ActivityMain) requireActivity()).rhyaSharedPreferences, ((ActivityMain) requireActivity()).rhyaAESManager, requireContext(), requireActivity());

                                    if (rhyaMusicDataVO != null && rhyaUserDataVO != null) {
                                        RhyaApplication.rhyaMusicDataVO = rhyaMusicDataVO;
                                        RhyaApplication.rhyaUserDataVO = rhyaUserDataVO;

                                        if (((ActivityMain) requireActivity()).rhyaCore.musicInfoReload(((ActivityMain) requireActivity()).rhyaAESManager,
                                                ((ActivityMain) requireActivity()).rhyaSharedPreferences,
                                                requireContext(),
                                                requireActivity(),
                                                auth)) {

                                            ((ActivityMain) requireActivity()).rhyaCore.InitSingerInfo();

                                            taskMusicInfo = getArrayListToMusicInfo();

                                            return "success";
                                        }
                                    }else {
                                        return null;
                                    }
                                }else {
                                    taskMusicInfo = getArrayListToMusicInfo();
                                    return "success";
                                }
                            } catch (NoSuchPaddingException | InvalidKeyException | IOException |
                                    IllegalBlockSizeException | BadPaddingException | NoSuchAlgorithmException |
                                    InvalidAlgorithmParameterException | ParseException | JSONException e) {

                                e.printStackTrace();

                                // 예외 처리
                                return "00008";
                            }
                        } catch (Exception ex) {
                            ex.printStackTrace();

                            return "00017";
                        }

                        return null;
                    }

                    @Override
                    protected void onPostExecute(String result) {
                        if (result != null) {
                            if (result.equals("00017")) {
                                ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(getContext(),
                                        "Unknown Error",
                                        "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00017)",
                                        "닫기",
                                        false,
                                        dialog -> {
                                            dialog.dismiss();
                                            dialogSub.dismiss();
                                            whitePanel.setVisibility(View.GONE);
                                            swipeRefreshLayout.setRefreshing(false);
                                        });

                                return;
                            }

                            if (result.equals("00008")) {
                                ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(getContext(),
                                        "Data Loading Error",
                                        "데이터 로딩 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00008)",
                                        "닫기",
                                        false,
                                        dialog -> {
                                            dialog.dismiss();
                                            dialogSub.dismiss();
                                            whitePanel.setVisibility(View.GONE);
                                            swipeRefreshLayout.setRefreshing(false);
                                        });

                                return;
                            }

                            songListLoadingIndex = 1;
                            isNoSongMoreView = false;

                            rhyaSongAdapter.setList(taskMusicInfo);

                            dialogSub.dismiss();
                            whitePanel.setVisibility(View.GONE);
                            swipeRefreshLayout.setRefreshing(false);
                        }else {
                            ((ActivityMain) requireActivity()).rhyaDialogManager.createDialog_Confirm(getContext(),
                                    "Data Loading Error",
                                    "데이터 로딩 과정에서 오류가 발생하였습니다. 다시 시도해주십시오. (00007)",
                                    "닫기",
                                    false,
                                    dialog -> {
                                        dialog.dismiss();
                                        dialogSub.dismiss();
                                        whitePanel.setVisibility(View.GONE);
                                        swipeRefreshLayout.setRefreshing(false);
                                    });
                        }
                    }
                }.execute(null));
    }

    public ArrayList<RhyaMusicInfoVO> getArrayListToMusicInfo() {
        if (isNoSongMoreView) {
            return null;
        }

        ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList = new ArrayList<>();
        ArrayList<RhyaMusicInfoVO> newRhyaMusicInfoVOArrayList = new ArrayList<>();

        for (String keys : RhyaApplication.rhyaMusicInfoVOHashMap.keySet()) {
            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(keys);

            rhyaMusicInfoVOArrayList.add(rhyaMusicInfoVO);
        }

        rhyaMusicInfoVOArrayList = rhyaMainUtils.sortRhyaMusicInfoVOArrayList(RhyaApplication.rhyaMusicDataVO.getSubscribeList(), rhyaMusicInfoVOArrayList);

        int nowSize = 0;

        newRhyaMusicInfoVOArrayList.add(0, new RhyaMusicInfoVO("main", null, null, null, null, null, null, null, null, null, null, null, 0));

        for (int i = 0; i < rhyaMusicInfoVOArrayList.size(); i ++) {
            newRhyaMusicInfoVOArrayList.add(rhyaMusicInfoVOArrayList.get(i));

            if (nowSize == (songListLoadingIndex + 20)) break;

            nowSize++;
        }

        if (newRhyaMusicInfoVOArrayList.size() == rhyaMusicInfoVOArrayList.size()) {
            isNoSongMoreView = true;
        }

        newRhyaMusicInfoVOArrayList.add(null);

        return newRhyaMusicInfoVOArrayList;
    }
}