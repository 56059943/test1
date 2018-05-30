struct ssl_ctx_st
{
	SSL_METHOD *method;
	STACK_OF(SSL_CIPHER) *cipher_list;
	/* same as above but sorted for lookup */
	STACK_OF(SSL_CIPHER) *cipher_list_by_id;
	struct x509_store_st /* X509_STORE */ *cert_store;
	struct lhash_st /* LHASH */ *sessions; /* a set of SSL_SESSIONs */
										   /* Most session-ids that will be cached, default is
										   * SSL_SESSION_CACHE_MAX_SIZE_DEFAULT. 0 is unlimited. */
	unsigned long session_cache_size;
	struct ssl_session_st *session_cache_head;
	struct ssl_session_st *session_cache_tail;
	/* This can have one of 2 values, ored together,
	* SSL_SESS_CACHE_CLIENT,
	* SSL_SESS_CACHE_SERVER,
	* Default is SSL_SESSION_CACHE_SERVER, which means only
	* SSL_accept which cache SSL_SESSIONS. */
	int session_cache_mode;
	/* If timeout is not 0, it is the default timeout value set
	* when SSL_new() is called.  This has been put in to make
	* life easier to set things up */
	long session_timeout;
	/* If this callback is not null, it will be called each
	* time a session id is added to the cache.  If this function
	* returns 1, it means that the callback will do a
	* SSL_SESSION_free() when it has finished using it.  Otherwise,
	* on 0, it means the callback has finished with it.
	* If remove_session_cb is not null, it will be called when
	* a session-id is removed from the cache.  After the call,
	* OpenSSL will SSL_SESSION_free() it. */
	int(*new_session_cb)(struct ssl_st *ssl, SSL_SESSION *sess);
	void(*remove_session_cb)(struct ssl_ctx_st *ctx, SSL_SESSION *sess);
	SSL_SESSION *(*get_session_cb)(struct ssl_st *ssl,
		unsigned char *data, int len, int *copy);
	struct
	{
		int sess_connect; /* SSL new conn - started */
		int sess_connect_renegotiate;/* SSL reneg - requested */
		int sess_connect_good; /* SSL new conne/reneg - finished */
		int sess_accept; /* SSL new accept - started */
		int sess_accept_renegotiate;/* SSL reneg - requested */
		int sess_accept_good; /* SSL accept/reneg - finished */
		int sess_miss;  /* session lookup misses  */
		int sess_timeout; /* reuse attempt on timeouted session */
		int sess_cache_full; /* session removed due to full cache */
		int sess_hit;  /* session reuse actually done */
		int sess_cb_hit; /* session-id that was not
						 * in the cache was
						 * passed back via the callback.  This
						 * indicates that the application is
						 * supplying session-id's from other
						 * processes - spooky :-) */
	} stats;
	int references;
	/* if defined, these override the X509_verify_cert() calls */
	int(*app_verify_callback)(X509_STORE_CTX *, void *);
	void *app_verify_arg;
	/* before OpenSSL 0.9.7, 'app_verify_arg' was ignored
	* ('app_verify_callback' was called with just one argument) */
	/* Default password callback. */
	pem_password_cb *default_passwd_callback;
	/* Default password callback user data. */
	void *default_passwd_callback_userdata;
	/* get client cert callback */
	int(*client_cert_cb)(SSL *ssl, X509 **x509, EVP_PKEY **pkey);
	CRYPTO_EX_DATA ex_data;
	const EVP_MD *rsa_md5;/* For SSLv2 - name is 'ssl2-md5' */
	const EVP_MD *md5; /* For SSLv3/TLSv1 'ssl3-md5' */
	const EVP_MD *sha1;   /* For SSLv3/TLSv1 'ssl3->sha1' */
	STACK_OF(X509) *extra_certs;
	STACK_OF(SSL_COMP) *comp_methods; /* stack of SSL_COMP, SSLv3/TLSv1 */

									  /* Default values used when no per-SSL value is defined follow */
	void(*info_callback)(const SSL *ssl, int type, int val); /* used if SSL's info_callback is NULL */
															 /* what we put in client cert requests */
	STACK_OF(X509_NAME) *client_CA;

	/* Default values to use in SSL structures follow (these are copied by SSL_new) */
	unsigned long options;
	unsigned long mode;
	long max_cert_list;
	struct cert_st /* CERT */ *cert;
	int read_ahead;
	/* callback that allows applications to peek at protocol messages */
	void(*msg_callback)(int write_p, int version, int content_type, const void *buf, size_t len, SSL *ssl, void *arg);
	void *msg_callback_arg;
	int verify_mode;
	int verify_depth;
	unsigned int sid_ctx_length;
	unsigned char sid_ctx[SSL_MAX_SID_CTX_LENGTH];
	int(*default_verify_callback)(int ok, X509_STORE_CTX *ctx); /* called 'verify_callback' in the SSL */
																/* Default generate session ID callback. */
	GEN_SESSION_CB generate_session_id;
	int purpose;  /* Purpose setting */
	int trust;  /* Trust setting */
	int quiet_shutdown;
};

typedef struct ssl_ctx_st SSL_CTX;
/* ssl/ssl_lib.h */
SSL_CTX *SSL_CTX_new(SSL_METHOD *meth)
{
	SSL_CTX *ret = NULL;

	if (meth == NULL)
	{
		SSLerr(SSL_F_SSL_CTX_NEW, SSL_R_NULL_SSL_METHOD_PASSED);
		return(NULL);
	}
	if (SSL_get_ex_data_X509_STORE_CTX_idx() < 0)
	{
		SSLerr(SSL_F_SSL_CTX_NEW, SSL_R_X509_VERIFICATION_SETUP_PROBLEMS);
		goto err;
	}
	// 分配上下文的内存空间
	ret = (SSL_CTX *)OPENSSL_malloc(sizeof(SSL_CTX));
	if (ret == NULL)
		goto err;
	memset(ret, 0, sizeof(SSL_CTX));

	// 初始化上下文的结构参数
	ret->method = meth;
	ret->cert_store = NULL;
	ret->session_cache_mode = SSL_SESS_CACHE_SERVER;
	ret->session_cache_size = SSL_SESSION_CACHE_MAX_SIZE_DEFAULT;
	ret->session_cache_head = NULL;
	ret->session_cache_tail = NULL;
	/* We take the system default */
	ret->session_timeout = meth->get_timeout();
	ret->new_session_cb = 0;
	ret->remove_session_cb = 0;
	ret->get_session_cb = 0;
	ret->generate_session_id = 0;
	memset((char *)&ret->stats, 0, sizeof(ret->stats));
	ret->references = 1;
	ret->quiet_shutdown = 0;
	/* ret->cipher=NULL;*/
	/* ret->s2->challenge=NULL;
	ret->master_key=NULL;
	ret->key_arg=NULL;
	ret->s2->conn_id=NULL; */
	ret->info_callback = NULL;
	ret->app_verify_callback = 0;
	ret->app_verify_arg = NULL;
	ret->max_cert_list = SSL_MAX_CERT_LIST_DEFAULT;
	ret->read_ahead = 0;
	ret->msg_callback = 0;
	ret->msg_callback_arg = NULL;
	ret->verify_mode = SSL_VERIFY_NONE;
	ret->verify_depth = -1; /* Don't impose a limit (but x509_lu.c does) */
	ret->sid_ctx_length = 0;
	ret->default_verify_callback = NULL;
	if ((ret->cert = ssl_cert_new()) == NULL)
		goto err;
	ret->default_passwd_callback = 0;
	ret->default_passwd_callback_userdata = NULL;
	ret->client_cert_cb = 0;
	ret->sessions = lh_new(LHASH_HASH_FN(SSL_SESSION_hash),
		LHASH_COMP_FN(SSL_SESSION_cmp));
	if (ret->sessions == NULL) goto err;
	ret->cert_store = X509_STORE_new();
	if (ret->cert_store == NULL) goto err;

	// 建立加密算法链表
	ssl_create_cipher_list(ret->method,
		&ret->cipher_list, &ret->cipher_list_by_id,
		SSL_DEFAULT_CIPHER_LIST);
	if (ret->cipher_list == NULL
		|| sk_SSL_CIPHER_num(ret->cipher_list) <= 0)
	{
		SSLerr(SSL_F_SSL_CTX_NEW, SSL_R_LIBRARY_HAS_NO_CIPHERS);
		goto err2;
	}

	// 定义上下文结构中HASH算法
	if ((ret->rsa_md5 = EVP_get_digestbyname("ssl2-md5")) == NULL)
	{
		SSLerr(SSL_F_SSL_CTX_NEW, SSL_R_UNABLE_TO_LOAD_SSL2_MD5_ROUTINES);
		goto err2;
	}
	if ((ret->md5 = EVP_get_digestbyname("ssl3-md5")) == NULL)
	{
		SSLerr(SSL_F_SSL_CTX_NEW, SSL_R_UNABLE_TO_LOAD_SSL3_MD5_ROUTINES);
		goto err2;
	}
	if ((ret->sha1 = EVP_get_digestbyname("ssl3-sha1")) == NULL)
	{
		SSLerr(SSL_F_SSL_CTX_NEW, SSL_R_UNABLE_TO_LOAD_SSL3_SHA1_ROUTINES);
		goto err2;
	}
	if ((ret->client_CA = sk_X509_NAME_new_null()) == NULL)
		goto err;

	CRYPTO_new_ex_data(CRYPTO_EX_INDEX_SSL_CTX, ret, &ret->ex_data);
	ret->extra_certs = NULL;
	// 压缩算法
	ret->comp_methods = SSL_COMP_get_compression_methods();
	return(ret);
err:
	SSLerr(SSL_F_SSL_CTX_NEW, ERR_R_MALLOC_FAILURE);
err2:
	if (ret != NULL) SSL_CTX_free(ret);
	return(NULL);
}