package kro.kr.rhya_network.utaiteplayer.utils;

import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;
import java.util.ArrayList;
import java.util.Arrays;

import javax.net.ssl.SSLSocket;
import javax.net.ssl.SSLSocketFactory;

public class RhyaTLSOnlySocketFactory extends SSLSocketFactory {
    private final SSLSocketFactory delegate;

    public RhyaTLSOnlySocketFactory(SSLSocketFactory delegate) {
        this.delegate = delegate;
    }

    @Override
    public String[] getDefaultCipherSuites() {

        return getPreferredDefaultCipherSuites(this.delegate);
    }

    @Override
    public String[] getSupportedCipherSuites() {
        return getPreferredSupportedCipherSuites(this.delegate);
    }



    @Override
    public Socket createSocket(Socket s, String host, int port, boolean autoClose) throws IOException {
        final Socket socket = this.delegate.createSocket(s, host, port, autoClose);

        ((SSLSocket)socket).setEnabledCipherSuites(getPreferredDefaultCipherSuites(delegate));
        ((SSLSocket)socket).setEnabledProtocols(getEnabledProtocols((SSLSocket)socket));

        return socket;
    }

    @Override
    public Socket createSocket(String s, int i) {
        return null;
    }

    @Override
    public Socket createSocket(String s, int i, InetAddress inetAddress, int i1) {
        return null;
    }

    @Override
    public Socket createSocket(InetAddress host, int port) throws IOException {
        final Socket socket = this.delegate.createSocket(host, port);

        ((SSLSocket)socket).setEnabledCipherSuites(getPreferredDefaultCipherSuites(delegate));
        ((SSLSocket) socket).setEnabledProtocols(getEnabledProtocols((SSLSocket)socket));

        return socket;
    }

    @Override
    public Socket createSocket(InetAddress address, int port, InetAddress localAddress, int localPort) throws IOException {
        final Socket socket = this.delegate.createSocket(address, port, localAddress, localPort);

        ((SSLSocket)socket).setEnabledCipherSuites(getPreferredDefaultCipherSuites(delegate));
        ((SSLSocket) socket).setEnabledProtocols(getEnabledProtocols((SSLSocket)socket));

        return socket;
    }

    private String[] getPreferredDefaultCipherSuites(SSLSocketFactory sslSocketFactory) {
        return getCipherSuites(sslSocketFactory.getDefaultCipherSuites());
    }

    private String[] getPreferredSupportedCipherSuites(SSLSocketFactory sslSocketFactory) {
        return getCipherSuites(sslSocketFactory.getSupportedCipherSuites());
    }

    private String[] getCipherSuites(String[] cipherSuites) {
        final ArrayList<String> suitesList = new ArrayList<>(Arrays.asList(cipherSuites));
        ArrayList<String> remove = new ArrayList<>();

        for (String cipherSuite : suitesList) {
            if (cipherSuite.contains("SSL")) {
                remove.add(cipherSuite);
            }
        }

        for (String name : remove) {
            suitesList.remove(name);
        }

        return suitesList.toArray(new String[0]);
    }

    private String[] getEnabledProtocols(SSLSocket socket) {
        final ArrayList<String> protocolList = new ArrayList<>(Arrays.asList(socket.getSupportedProtocols()));
        ArrayList<String> remove = new ArrayList<>();

        for (String protocol : protocolList) {
            if (protocol.contains("SSL")) {
                remove.add(protocol);
            }
        }

        for (String name : remove) {
            protocolList.remove(name);
        }



        return protocolList.toArray(new String[0]);
    }
}
