package kro.kr.rhya_network.utaiteplayer.fragment;

import android.content.ContentValues;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ImageButton;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.ItemTouchHelper;
import androidx.recyclerview.widget.LinearLayoutManager;
import androidx.recyclerview.widget.RecyclerView;

import org.json.JSONObject;

import java.net.URLEncoder;
import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.adapter.RhyaMyPlayListInfoAdapter;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaDialogManagerNew;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaItemTouchHelperCallBack;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaMusicInfoVO;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaPlayList;
import kro.kr.rhya_network.utaiteplayer.utils.StickyHeaderItemDecoration;

public class MyPlayListInfoFragment extends Fragment {
    // UI Object
    private TextView title;
    private RecyclerView recyclerView;
    // Adapter
    public RhyaMyPlayListInfoAdapter rhyaMyPlayListInfoAdapter;
    // RhyaPlayList
    public RhyaPlayList rhyaPlayList;




    public MyPlayListInfoFragment() {
        // Required empty public constructor
    }

    public static MyPlayListInfoFragment newInstance() {
        return new MyPlayListInfoFragment();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_my_playlist_info, container, false);
    }

    @Override
    public void onViewCreated(@NonNull View view, @Nullable Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);

        // UI Object
        ImageButton backButton = view.findViewById(R.id.backButton);
        title = view.findViewById(R.id.title);
        TextView editTextViewButton = view.findViewById(R.id.editTextViewButton);
        recyclerView = view.findViewById(R.id.recyclerView);

        // UI Object setting
        rhyaMyPlayListInfoAdapter = new RhyaMyPlayListInfoAdapter(requireActivity(), this);
        StickyHeaderItemDecoration stickyHeaderItemDecoration = new StickyHeaderItemDecoration(new StickyHeaderItemDecoration.SectionCallback() {
            @Override
            public boolean isSection(int position) {
                return rhyaMyPlayListInfoAdapter.isHeader(position);
            }

            @Nullable
            @Override
            public View getHeaderLayoutView(@NonNull RecyclerView list, int position) {
                return rhyaMyPlayListInfoAdapter.getHeaderLayoutView(list, position);
            }
        });

        // ItemTouchHelper
        RhyaItemTouchHelperCallBack rhyaItemTouchHelperCallBack = new RhyaItemTouchHelperCallBack(rhyaMyPlayListInfoAdapter, rhyaMyPlayListInfoAdapter);
        ItemTouchHelper itemTouchHelper = new ItemTouchHelper(rhyaItemTouchHelperCallBack);
        itemTouchHelper.attachToRecyclerView(recyclerView);

        LinearLayoutManager linearLayoutManager = new LinearLayoutManager(requireContext());
        linearLayoutManager.setOrientation(LinearLayoutManager.VERTICAL);
        recyclerView.setLayoutManager(linearLayoutManager);
        recyclerView.setAdapter(rhyaMyPlayListInfoAdapter);
        recyclerView.addItemDecoration(stickyHeaderItemDecoration);

        // Listener
        backButton.setOnClickListener(v -> { ((ActivityMain) requireActivity()).showMyPlayListFragment(); ((ActivityMain) requireActivity()).setStatePlayListSongsRemoveButton(false); });
        editTextViewButton.setOnClickListener(v -> {
            if (rhyaPlayList != null) {
                new RhyaDialogManagerNew(requireActivity(), true, 5).setAndShowDialogSettingType_5("플레이리스트 수정", rhyaPlayList.getName(), rhyaPlayList.getImageType(), playListOkButtonListener);
            }
        });
    }

    private final RhyaDialogManagerNew.DialogListener_PlayListOkButtonListener playListOkButtonListener = (dialog, title, image) -> {
        dialog.dismiss();

        // 서버 접속
        new RhyaAsyncTask<String, String>() {

            @Override
            protected void onPreExecute() {
                ((ActivityMain) requireActivity()).showTaskDialog();
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    String authToken = ((ActivityMain) requireActivity()).rhyaCore.getAutoLogin(((ActivityMain) requireActivity()).rhyaSharedPreferences, requireActivity());
                    RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                    ContentValues urlParam = new ContentValues();

                    urlParam.put("mode", "14");
                    urlParam.put("index", "2");
                    urlParam.put("auth", authToken);
                    urlParam.put("value1", "name");
                    urlParam.put("value2", rhyaPlayList.getUuid());
                    urlParam.put("value3", URLEncoder.encode(URLEncoder.encode(title, "UTF-8"), "UTF-8"));

                    try {
                        JSONObject jsonObject = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) requireActivity()).rhyaCore.MAIN_URL, urlParam));

                        urlParam.put("mode", "14");
                        urlParam.put("index", "2");
                        urlParam.put("auth", authToken);
                        urlParam.put("value1", "image");
                        urlParam.put("value2", rhyaPlayList.getUuid());
                        if (image == -1) {
                            urlParam.put("value3", URLEncoder.encode("_IMAGE_TYPE_".concat(String.valueOf(rhyaPlayList.getImageType())), "UTF-8"));
                        }else {
                            urlParam.put("value3", URLEncoder.encode("_IMAGE_TYPE_".concat(String.valueOf(image)), "UTF-8"));
                        }

                        JSONObject jsonObject2 = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) requireActivity()).rhyaCore.MAIN_URL, urlParam));

                        String result = jsonObject.getString("result");
                        String result2 = jsonObject2.getString("result");

                        if (result.equals("success") && result2.equals("success")) {
                            ((ActivityMain) requireActivity()).rhyaPlayList.setName(title);
                            if (image != -1) {
                                ((ActivityMain) requireActivity()).rhyaPlayList.setImageType(image);
                            }

                            // 데이터 적용
                            ((MyPlayListFragment) ((ActivityMain) requireActivity()).myPlayListFragment).isReload = true;

                            return "success";
                        }else {
                            return "fail";
                        }
                    }catch (Exception ex) {
                        ex.printStackTrace();

                        return "error2";
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();

                    return "error1";
                }
            }

            @Override
            protected void onPostExecute(String result) {
                switch (result) {

                    case "error1": {
                        Toast.makeText(requireActivity(), "서버 접속 중에 오류가 발생하였습니다. (00082)", Toast.LENGTH_SHORT).show();

                        break;
                    }

                    case "error2": {
                        Toast.makeText(requireActivity(), "JSON 구문분석 중 오류가 발생하였습니다. (00083)", Toast.LENGTH_SHORT).show();

                        break;
                    }

                    case "fail": {
                        Toast.makeText(requireActivity(), "서버가 접근을 명령을 거부하였습니다. (00084)", Toast.LENGTH_SHORT).show();

                        break;
                    }

                    case "success": {
                        ((ActivityMain) requireActivity()).dismissTaskDialog();
                        onReload();

                        break;
                    }
                }

                ((ActivityMain) requireActivity()).dismissTaskDialog();
            }
        }.execute(null);
    };


    @Override
    public void onResume() {
        super.onResume();

        onReload();
    }

    private void onReload() {
        // Select playlist
        rhyaPlayList = ((ActivityMain) requireActivity()).rhyaPlayList;

        if (((ActivityMain) requireActivity()).isPlayListScrollToTop) {
            ((ActivityMain) requireActivity()).isPlayListScrollToTop = false;

            recyclerView.scrollToPosition(0);
        }

        title.setText(rhyaPlayList.getName());

        new RhyaAsyncTask<String, String>() {
            private ArrayList<RhyaMusicInfoVO> rhyaMusicInfoVOArrayList;

            @Override
            protected void onPreExecute() {
                rhyaMusicInfoVOArrayList = new ArrayList<>();

                ((ActivityMain) requireActivity()).showTaskDialog();

                rhyaMusicInfoVOArrayList.clear();
                rhyaMusicInfoVOArrayList.add(null);
                rhyaMusicInfoVOArrayList.add(new RhyaMusicInfoVO("_BUTTON_HEADER_", null,null,null,null,null,null,null,null,null,null,null,0));
                rhyaMyPlayListInfoAdapter.setList(rhyaMusicInfoVOArrayList);
            }

            @Override
            protected String doInBackground(String arg) {
                try {
                    for (int index = 0; index < rhyaPlayList.getMusicList().size(); index ++) {
                        try {
                            String uuid = rhyaPlayList.getMusicList().get(index);
                            RhyaMusicInfoVO rhyaMusicInfoVO = RhyaApplication.rhyaMusicInfoVOHashMap.get(uuid);
                            assert rhyaMusicInfoVO != null;

                            rhyaMusicInfoVOArrayList.add(new RhyaMusicInfoVO(
                                    rhyaMusicInfoVO.getUuid(),
                                    rhyaMusicInfoVO.getName(),
                                    rhyaMusicInfoVO.getTime(),
                                    rhyaMusicInfoVO.getLyrics(),
                                    rhyaMusicInfoVO.getSinger(),
                                    rhyaMusicInfoVO.getSingerUuid(),
                                    rhyaMusicInfoVO.getSingerImage(),
                                    rhyaMusicInfoVO.getSongWriter(),
                                    rhyaMusicInfoVO.getImage(),
                                    rhyaMusicInfoVO.getMp3(),
                                    rhyaMusicInfoVO.getType(),
                                    rhyaMusicInfoVO.getDate(),
                                    rhyaMusicInfoVO.getVersion()));
                        }catch (Exception ex) {
                            ex.printStackTrace();
                        }
                    }
                }catch (Exception ex) {
                    ex.printStackTrace();
                }

                return null;
            }

            @Override
            protected void onPostExecute(String result) {
                rhyaMyPlayListInfoAdapter.setList(rhyaMusicInfoVOArrayList);

                ((ActivityMain) requireActivity()).dismissTaskDialog();
            }
        }.execute(null);
    }
}
