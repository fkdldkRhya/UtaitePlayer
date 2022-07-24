package kro.kr.rhya_network.utaiteplayer.adapter;

import android.annotation.SuppressLint;
import android.app.Dialog;
import android.content.ContentValues;
import android.content.Context;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.recyclerview.widget.RecyclerView;

import com.bumptech.glide.Glide;
import com.github.siyamed.shapeimageview.RoundedImageView;

import org.json.JSONObject;

import java.net.URLDecoder;
import java.util.ArrayList;

import kro.kr.rhya_network.utaiteplayer.R;
import kro.kr.rhya_network.utaiteplayer.activity.ActivityMain;
import kro.kr.rhya_network.utaiteplayer.core.RhyaApplication;
import kro.kr.rhya_network.utaiteplayer.fragment.HomeFragment;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaAsyncTask;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaHttpsConnection;
import kro.kr.rhya_network.utaiteplayer.utils.RhyaSingerDataVO;

public class RhyaMySubscribeAdapter extends RecyclerView.Adapter<RhyaMySubscribeAdapter.ViewHolder> {
    private ArrayList<RhyaSingerDataVO> listMain;
    private final Fragment fragment;
    private Context context;


    public RhyaMySubscribeAdapter(Fragment fragment) {
        this.fragment = fragment;
    }

    @NonNull
    @Override
    public RhyaMySubscribeAdapter.ViewHolder onCreateViewHolder(@NonNull ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.item_subscribe, parent, false);
        context = view.getContext();

        return new RhyaMySubscribeAdapter.ViewHolder(view);
    }

    @Override
    public void onBindViewHolder(@NonNull RhyaMySubscribeAdapter.ViewHolder holder, int position) {
        holder.onBind(listMain.get(position));
    }

    @SuppressLint("NotifyDataSetChanged")
    public void setList(ArrayList<RhyaSingerDataVO> list) {
        this.listMain = list;

        notifyDataSetChanged();
    }

    @Override
    public int getItemCount() {
        return listMain.size();
    }

    @Override
    public int getItemViewType(int position) {
        return position;
    }

    class ViewHolder extends RecyclerView.ViewHolder {
        private final RoundedImageView singerImage;
        private final TextView singerName;
        private final Button csButton;


        public ViewHolder(@NonNull View itemView) {
            super(itemView);

            singerImage = itemView.findViewById(R.id.imageView);
            singerName = itemView.findViewById(R.id.name);
            csButton = itemView.findViewById(R.id.button);
        }

        public void onBind(RhyaSingerDataVO rhyaSingerDataVO) {
            singerName.setText(rhyaSingerDataVO.getName());

            Glide.with(context)
                    .load(rhyaSingerDataVO.getImage())
                    .placeholder(R.drawable.img_load_error)
                    .error(R.drawable.img_load_error)
                    .fallback(R.drawable.img_load_error)
                    .into(singerImage);

            csButton.setOnClickListener(v -> {
                try {
                    ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Task(fragment.getContext(),
                            "구독 취소 중...",
                            false,
                            dialog -> new RhyaAsyncTask<String, String>() {
                                @Override
                                protected void onPreExecute() {
                                }

                                @Override
                                protected String doInBackground(String arg) {
                                    try {
                                        RhyaHttpsConnection rhyaHttpsConnection = new RhyaHttpsConnection();
                                        ContentValues urlParam = new ContentValues();
                                        urlParam.put("auth", ((ActivityMain) fragment.requireActivity()).rhyaCore.getAutoLogin(((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences, fragment.getContext()));
                                        urlParam.put("mode", 13);
                                        urlParam.put("index", 1);
                                        urlParam.put("value", rhyaSingerDataVO.getUuid());

                                        JSONObject result = new JSONObject(rhyaHttpsConnection.request(((ActivityMain) fragment.requireActivity()).rhyaCore.MAIN_URL, urlParam));

                                        if (result.getString("result").equals("success")) {
                                            return "success";
                                        }else {
                                            return URLDecoder.decode(result.getString("message"), "UTF-8");
                                        }
                                    }catch (Exception ex) {
                                        ex.printStackTrace();

                                        return null;
                                    }
                                }

                                @Override
                                protected void onPostExecute(String result) {
                                    try {
                                        if (result != null) {
                                            if (result.equals("success")) {
                                                int pos = getBindingAdapterPosition();

                                                listMain.remove(pos);

                                                ((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences.removeString(((ActivityMain) fragment.requireActivity()).rhyaSharedPreferences.SHARED_PREFERENCES_USER_METADATA_CHANGE, fragment.requireContext());

                                                ((HomeFragment) fragment).songListLoadingIndex = 0;
                                                ((HomeFragment) fragment).isNoSongMoreView = false;
                                                ((HomeFragment) fragment).reloadData(true);

                                                RhyaApplication.rhyaMusicDataVO.getSubscribeList().remove(rhyaSingerDataVO);

                                                notifyItemRemoved(pos);

                                                dialog.dismiss();
                                            }else {
                                                dialog.dismiss();

                                                ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(context,
                                                        "Server Connection Error",
                                                        result,
                                                        "종료",
                                                        true,
                                                        Dialog::dismiss);
                                            }
                                        }else {
                                            dialog.dismiss();

                                            ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(context,
                                                    "Unknown Error",
                                                    "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00016)",
                                                    "닫기",
                                                    true,
                                                    Dialog::dismiss);
                                        }
                                    }catch (Exception ex) {
                                        ex.printStackTrace();

                                        dialog.dismiss();

                                        ((ActivityMain) fragment.requireActivity()).rhyaDialogManager.createDialog_Confirm(context,
                                                "Unknown Error",
                                                "알 수 없는 오류가 발생하였습니다. 다시 시도해주십시오. (00015)",
                                                "종료",
                                                true,
                                                Dialog::dismiss);
                                    }
                                }
                            }.execute(null));
                }catch (Exception ex) {
                    ex.printStackTrace();
                }
            });
        }
    }
}
